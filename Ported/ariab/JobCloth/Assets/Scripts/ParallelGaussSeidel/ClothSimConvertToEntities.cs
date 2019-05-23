using System;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class ClothConvertBarrierSystem : EntityCommandBufferSystem
{}

public class ClothSimConvertToEntities : JobComponentSystem
{
    private static readonly int kMaxClusters = 12;
    
    private EntityQuery m_clothToConvertGroup;
    private EntityQuery m_clothConvertedGroup;
    private EntityArchetype m_simPointArchetype;
    private EntityArchetype m_constraintArchetype;

    private ClothConvertBarrierSystem m_barrier;

    [BurstCompile]
    [RequireComponentTag(typeof(ClothGarment))]
    struct CopyVerticesToSimPointsJob : IJobForEachWithEntity<Translation, PreviousPosition, ProjectedPosition>
    {
        [DeallocateOnJobCompletion]
        [ReadOnly]
        public NativeArray<Vector3> Vertices;
        
        public void Execute(Entity entity, int index, ref Translation current, ref PreviousPosition previous, ref ProjectedPosition projected)
        {
            var currentVertex = Vertices[index];
            var vertexPosition = new float3(currentVertex.x, currentVertex.y, currentVertex.z);
            
            current = new Translation
            {
                Value = vertexPosition
            };
            
            previous = new PreviousPosition
            {
                Value = vertexPosition
            };
            
            projected = new ProjectedPosition
            {
                Value = vertexPosition
            };
        }
    }

    struct RemoveConvertClothTagJob : IJobForEachWithEntity<ConvertToCloth>
    {
        public EntityCommandBuffer.Concurrent Cmds;
        
        public void Execute(Entity entity, int index, ref ConvertToCloth _)
        {
            Cmds.RemoveComponent<ConvertToCloth>(index, entity);
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {        
        var numClothToConvert = m_clothToConvertGroup.CalculateLength();
        if (numClothToConvert <= 0)
            return inputDeps;
        
        ClothSimSystem.SetStaticGarmentListCapacity(numClothToConvert);
        
        var convertToClothEntities = m_clothToConvertGroup.ToEntityArray(Allocator.TempJob);

        for (int garmentIndex = 0; garmentIndex < numClothToConvert; ++garmentIndex)
        {
            var entity = convertToClothEntities[garmentIndex];
            var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
            var meshVertices = renderMesh.mesh.vertices;
            var simPointCount = meshVertices.Length;
            
            // create garment sim points
            var simPointEntities = new NativeArray<Entity>(simPointCount, Allocator.TempJob);
            EntityManager.CreateEntity(m_simPointArchetype, simPointEntities);
            
            // create new instance of the mesh
            // If we have two of the same cloth that needs to be converted
            var newMesh = new Mesh();
            newMesh.vertices = renderMesh.mesh.vertices;
            newMesh.normals = renderMesh.mesh.normals;
            newMesh.tangents = renderMesh.mesh.tangents;
            newMesh.triangles = renderMesh.mesh.triangles;
            newMesh.MarkDynamic();
                    
            EntityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = newMesh,
                material = renderMesh.material,
                subMesh = renderMesh.subMesh,
                castShadows = renderMesh.castShadows,
                layer = renderMesh.layer,
                receiveShadows = renderMesh.receiveShadows
            });
            
            // set garment meshes
            // todo: turn this into job
            for (int i = 0; i < simPointCount; ++i)
            {               
                // todo: this is ugly, but currently necessary
                // each cloth point accesses a NativeArray<float4x4> during the sim jobs to get its own LocalToWorld matrix
                // since we cannot currently access the SharedComponentData in a job, we need to store that index once per point
                // but we still need the SharedComponentData so we can filter on individual garments
                EntityManager.SetSharedComponentData(simPointEntities[i], new ClothGarment { GarmentIndex = (byte)(garmentIndex+1) });
                EntityManager.SetComponentData(simPointEntities[i], new ClothGarmentIndex  { GarmentIndex = (byte)(garmentIndex+1) });

                var normals = newMesh.normals;
                if (normals[i].y > .9f && meshVertices[i].y > .3f)
                {
                    EntityManager.SetComponentData(simPointEntities[i], new PinWeight{Value = 1});
                }
            }

            var localToWorld = EntityManager.GetComponentData<LocalToWorld>(entity).Value;
            ClothSimSystem.AddGarmentDataToStaticList(newMesh, math.inverse(localToWorld));
            
            simPointEntities.Dispose();
        }
        
        var allSetPositionsHandle = new JobHandle();
        for (int garmentIndex = 0; garmentIndex < numClothToConvert; ++garmentIndex)
        {
            var entity = convertToClothEntities[garmentIndex];
            var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
            var meshVertices = renderMesh.mesh.vertices;
            var simPointCount = meshVertices.Length;
            
            // copy Vector3[] from Mesh to a NativeArray so we can set the values in the job
            var verticesAsNativeArray = new NativeArray<Vector3>(simPointCount, Allocator.TempJob);
            verticesAsNativeArray.CopyFrom(meshVertices);
           
            // set garment sim point positions
            m_clothConvertedGroup.SetFilter(new ClothGarment{ GarmentIndex = (byte)(garmentIndex+1) });
            var copyToSimPointsJob = new CopyVerticesToSimPointsJob
            {
                Vertices = verticesAsNativeArray
            };
            var copyHandle = copyToSimPointsJob.Schedule(m_clothConvertedGroup, allSetPositionsHandle);
            allSetPositionsHandle = JobHandle.CombineDependencies(allSetPositionsHandle, copyHandle);          
        }
        
        // todo: remove sync point
        // sync point put here because below distance constraint init code creates entities
        // probably just need to create all the entities before we run jobs
        allSetPositionsHandle.Complete();
                
        #region Distance Constraint Initialization
        // todo: optimize this
        for (int garmentIndex = 0; garmentIndex < numClothToConvert; ++garmentIndex)
        {
            var entity = convertToClothEntities[garmentIndex];
            var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
        
            var barLookup = new HashSet<Vector2Int>();
            var triangles = renderMesh.mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int pair = new Vector2Int();
                    pair.x = triangles[i + j];
                    pair.y = triangles[i + (j + 1) % 3];
                    if (pair.x > pair.y)
                    {
                        var newY = pair.x;
                        pair.x = pair.y;
                        pair.y = newY;
                    }
        
                    if (barLookup.Contains(pair) == false)
                    {
                        barLookup.Add(pair);
                    }
                }
            }
        
            var barList = new List<Vector2Int>(barLookup);
            var constraintCount = barList.Count;
        
            var constraintEntities = new NativeArray<Entity>(constraintCount, Allocator.Temp);
            EntityManager.CreateEntity(m_constraintArchetype, constraintEntities);

            // todo: eww allocations
            // We've already allocated and deallocated these same arrays above
            // Store them in a list, use them here, then deallocate them after
            m_clothConvertedGroup.SetFilter(new ClothGarment{ GarmentIndex = (byte)(garmentIndex+1)});
            var simPointEntities = m_clothConvertedGroup.ToEntityArray(Allocator.TempJob);
        
            for (int constraintIndex = 0; constraintIndex < barList.Count; constraintIndex++)
            {
                var constraintEntity = constraintEntities[constraintIndex];
                
                Vector2Int pair = barList[constraintIndex];
                Vector3 p1 = renderMesh.mesh.vertices[pair.x];
                Vector3 p2 = renderMesh.mesh.vertices[pair.y];

                EntityManager.SetComponentData(constraintEntity, 
                    new DistanceConstraint
                {
                    A = simPointEntities[pair.x],
                    B = simPointEntities[pair.y],
                    RestLengthSqr = (p2 - p1).sqrMagnitude
                });

                // Initialize all clusters to -1 for clustering algorithm below
                EntityManager.SetSharedComponentData(constraintEntity, new ConstraintCluster {ClusterIndex = -1});
            }
            
            // make list of constraints adjacent to each point
            var adjacentConstraintsPerPoint = new List<List<int>>(simPointEntities.Length);
            {
                // allocate our lists
                // todo: gross
                for (int listIndex = 0; listIndex < adjacentConstraintsPerPoint.Capacity; ++listIndex)
                {
                    adjacentConstraintsPerPoint.Add(new List<int>(kMaxClusters));
                }

                // iterate through all constraints adding to sim point lists as necessary
                for (int constraintIndex = 0; constraintIndex < constraintEntities.Length; ++constraintIndex)
                {
                    var constraintSimPoints = barList[constraintIndex];
                    adjacentConstraintsPerPoint[constraintSimPoints.x].Add(constraintIndex);
                    adjacentConstraintsPerPoint[constraintSimPoints.y].Add(constraintIndex);
                }
            }
            
            // make set of constraints adjacent to each constraint
            var adjacentConstraintsPerConstraint = new List<HashSet<int>>(constraintCount);
            {
                // allocate our lists
                // todo: gross
                for (int listIndex = 0; listIndex < adjacentConstraintsPerConstraint.Capacity; ++listIndex)
                {
                    adjacentConstraintsPerConstraint.Add(new HashSet<int>());
                }
                
                // iterate through all constraints adding to adjacent constraint set
                for (int constraintIndex = 0; constraintIndex < constraintEntities.Length; ++constraintIndex)
                {
                    var constraintSimPoints = barList[constraintIndex];

                    var adjacentConstraintsA = adjacentConstraintsPerPoint[constraintSimPoints.x];
                    for (int adjacentIndex = 0; adjacentIndex < adjacentConstraintsA.Count; ++adjacentIndex)
                    {
                        adjacentConstraintsPerConstraint[constraintIndex].Add(adjacentConstraintsA[adjacentIndex]);
                    }
                    
                    var adjacentConstraintsB = adjacentConstraintsPerPoint[constraintSimPoints.y];
                    for (int adjacentIndex = 0; adjacentIndex < adjacentConstraintsB.Count; ++adjacentIndex)
                    {
                        adjacentConstraintsPerConstraint[constraintIndex].Add(adjacentConstraintsB[adjacentIndex]);
                    }
                }
                
            }
            
            // color
            {
                // Set first constraint color to 0
                EntityManager.SetSharedComponentData(constraintEntities[0], new ConstraintCluster {ClusterIndex = 0});

                // index corresponds to cluster index, value = 0 if not found and 1 if found
                var foundColorInAdjacent = new NativeArray<byte>(kMaxClusters, Allocator.Temp);
                
                // Color all other constraints
                for (int constraintIndex = 1; constraintIndex < constraintEntities.Length; ++constraintIndex)
                {
                    var constraintEntity = constraintEntities[constraintIndex];
                    
                    // iterate through all adjacent constraints and fill list of found colors
                    var adjacentSet = adjacentConstraintsPerConstraint[constraintIndex];
                    foreach (var adjacentIndex in adjacentSet)
                    {
                        var adjacentEntity = constraintEntities[adjacentIndex];
                        var color = EntityManager.GetSharedComponentData<ConstraintCluster>(adjacentEntity);
                        if(color.ClusterIndex != -1)
                            foundColorInAdjacent[color.ClusterIndex] = 1;
                    }
                    
                    // find first color that has not been found in an adjacent constraint
                    var selectedColor = -1;
                    for (int i = 0; i < foundColorInAdjacent.Length; ++i)
                    {
                        if (foundColorInAdjacent[i] == 0)
                        {
                            selectedColor = i;
                            break;
                        }
                    }
                    
                    // reset values to 0
                    for (int i = 0; i < foundColorInAdjacent.Length; ++i)
                    {
                        foundColorInAdjacent[i] = 0;
                    }
                    
                    if(selectedColor == -1)
                        throw new InvalidDataException($"Failed to find a color for a constraint in the cloth garment. Need to increase kMaxClusters (currently {kMaxClusters})");
                    
                    EntityManager.SetSharedComponentData(constraintEntity, new ConstraintCluster { ClusterIndex = selectedColor});                   
                }
                
                foundColorInAdjacent.Dispose();
            }
                            
            constraintEntities.Dispose();
            simPointEntities.Dispose();
        }
        
        #endregion
        
        convertToClothEntities.Dispose(); 

        // remove ConvertToCloth from garment
        var removeConvertJob = new RemoveConvertClothTagJob()
        {
            Cmds = m_barrier.CreateCommandBuffer().ToConcurrent()
        };
        var removeHandle = removeConvertJob.Schedule(m_clothToConvertGroup, allSetPositionsHandle);

        var outHandle = removeHandle;     
        return outHandle;
    }

    protected override void OnCreateManager()
    {
        m_clothToConvertGroup = GetEntityQuery(
        typeof(ConvertToCloth), 
        typeof(Translation), 
        typeof(RenderMesh));
        
        m_clothConvertedGroup = GetEntityQuery(
        typeof(ClothGarment), 
        typeof(Translation), 
        typeof(PreviousPosition), 
        typeof(ProjectedPosition),           
        typeof(PinWeight),
        typeof(ClothGarmentIndex));
        
        m_simPointArchetype = EntityManager.CreateArchetype(
            typeof(ClothGarment), 
            typeof(Translation), 
            typeof(PreviousPosition), 
            typeof(ProjectedPosition), 
            typeof(PinWeight),
            typeof(ClothGarmentIndex));
        
        m_constraintArchetype = EntityManager.CreateArchetype(
            typeof(DistanceConstraint), 
            typeof(ConstraintCluster));

        m_barrier = World.GetOrCreateSystem<ClothConvertBarrierSystem>();
    }
}
