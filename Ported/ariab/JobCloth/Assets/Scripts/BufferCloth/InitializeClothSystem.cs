using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

//[UpdateInGroup(typeof(InitializationSystemGroup))]
public unsafe class InitializeClothSystem : GameObjectConversionSystem
{
    enum HierarchicalClassification
    {
        Coarse = 0,
        Fine = 1
    }

    private List<List<int>> CreateNeighborListFromMesh(Mesh mesh)
    {
        // Init barlist
        var neighborList = new List<List<int>>(mesh.vertexCount);
        for (int i = 0; i < mesh.vertexCount; ++i)
            neighborList.Add(new List<int>(4));
        
        var barLookup = new HashSet<Vector2Int>();
        var triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2Int pair = new Vector2Int
                {
                    x = triangles[i + j],
                    y = triangles[i + (j + 1) % 3]
                };
                
                if (pair.x > pair.y)
                {
                    var newY = pair.x;
                    pair.x = pair.y;
                    pair.y = newY;
                }
        
                if (barLookup.Contains(pair) == false)
                {
                    barLookup.Add(pair);
                    neighborList[pair.x].Add(pair.y);
                    neighborList[pair.y].Add(pair.x);
                }
            }
        }

        return neighborList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="originalMesh"></param>
    /// <param name="localToGlobalIndices">Length is the # of vertices in this specific level. Value is the index from the original mesh.</param>
    /// <param name="neighborList">Outer count is the # of vertices in this level, inner count is number of neighbors on this level.</param>
    private Entity CreateLevelEntityWithCurrentData(
        int levelIndex, 
        Mesh originalMesh,
        NativeArray<int> localToGlobalIndices, 
        NativeHashMap<int, int> globalToLocalIndices, 
        List<List<int>> neighborList)
    {
        var hierarchicalLevelArchetype = DstEntityManager.CreateArchetype(
            typeof(ClothProjectedPosition), 
            typeof(ClothCurrentPosition), 
            typeof(ClothPreviousPosition), 
            typeof(ClothDistanceConstraint), 
            typeof(ClothPinWeight),
            typeof(ClothHierarchyDepth),
            typeof(ClothHierarchicalParentIndexAndWeights),
            typeof(ClothHierarchyParentEntity)
        );
        var levelEntity = DstEntityManager.CreateEntity(hierarchicalLevelArchetype);
        var vertexCount = localToGlobalIndices.Length;
        
        // Grab position information from the original mesh
        var thisLevelPositions = new NativeArray<float3>(vertexCount, Allocator.Temp);
        var thisLevelNormals = new NativeArray<float3>(vertexCount, Allocator.Temp);
        for (int i = 0; i < vertexCount; ++i)
        {
            thisLevelPositions[i] = originalMesh.vertices[localToGlobalIndices[i]];
            thisLevelNormals[i]   = originalMesh.normals[localToGlobalIndices[i]];
        }

        // Copy initial vert data to buffer
        var projectedPositionBuffer = DstEntityManager.GetBuffer<ClothProjectedPosition>(levelEntity);
        projectedPositionBuffer.Reserve(vertexCount);
        projectedPositionBuffer.CopyFrom(thisLevelPositions.Reinterpret<float3, ClothProjectedPosition>());

        var currentPositionBuffer = DstEntityManager.GetBuffer<ClothCurrentPosition>(levelEntity);
        currentPositionBuffer.Reserve(vertexCount);
        currentPositionBuffer.CopyFrom(thisLevelPositions.Reinterpret<float3, ClothCurrentPosition>());

        var previousPositionBuffer = DstEntityManager.GetBuffer<ClothPreviousPosition>(levelEntity);
        previousPositionBuffer.Reserve(vertexCount);
        previousPositionBuffer.CopyFrom(thisLevelPositions.Reinterpret<float3, ClothPreviousPosition>());
        
        // Initialize size of parent index/weights buffer
        var parentInfoBuffer = DstEntityManager.GetBuffer<ClothHierarchicalParentIndexAndWeights>(levelEntity);
        parentInfoBuffer.ResizeUninitialized(vertexCount);
        
        // Add pin weights
        var pinWeightBuffer = DstEntityManager.GetBuffer<ClothPinWeight>(levelEntity);
        pinWeightBuffer.Reserve(vertexCount);

        for (int i = 0; i < vertexCount; ++i)
        {
            //if (thisLevelNormals[i].y > .9f && thisLevelPositions[i].y > .3f)
            if (math.abs(thisLevelPositions[i].x) > 2.25f)
                pinWeightBuffer.Add(new ClothPinWeight {InvPinWeight = 0.0f});
            else
                pinWeightBuffer.Add(new ClothPinWeight {InvPinWeight = 1.0f});
        }
        
        // Reduce neighbor list to set of unique constraints
        var barLookup = new HashSet<Vector2Int>();
        for (int currentGlobalIndex = 0; currentGlobalIndex < neighborList.Count; currentGlobalIndex++)
        {
            var currentNeighborList = neighborList[currentGlobalIndex];
            for (int indexOfGlobalNeighborIndex = 0; indexOfGlobalNeighborIndex < currentNeighborList.Count; indexOfGlobalNeighborIndex++)
            {
                // Indices here are indices inside the current level (not indices in the original mesh)
                Vector2Int pair = new Vector2Int
                {
                    x = globalToLocalIndices[currentGlobalIndex],
                    y = globalToLocalIndices[currentNeighborList[indexOfGlobalNeighborIndex]]
                };
                
                // ensures uniqueness
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
        
        // Add constraints to the entity
        var barList = new List<Vector2Int>(barLookup);
        var constraintCount = barList.Count;
        
        var constraintsBuffer = DstEntityManager.GetBuffer<ClothDistanceConstraint>(levelEntity);
        constraintsBuffer.Reserve(constraintCount);

        for (int i = 0; i < constraintCount; ++i)
        {
            var p1 = thisLevelPositions[barList[i].x];
            var p2 = thisLevelPositions[barList[i].y];
            
            constraintsBuffer.Add(new ClothDistanceConstraint
            {
                RestLengthSqr = math.lengthsq(p2 - p1),
                VertexA = barList[i].x,
                VertexB = barList[i].y
            });
        }
        
        // Set our hierarchy depth
        DstEntityManager.SetSharedComponentData(levelEntity, new ClothHierarchyDepth{Level = levelIndex});
        return levelEntity;
    }

    private Entity CreateHierarchy(int numberOfHierarchyLevels, Mesh sourceMesh)
    {
        // "Local" indices are the indices into the current level's vertex list
        // "Global" indices are the indices into the original mesh's vertex list
        // For L = 0, these are identical but this changes as we generate parent subsets for L > 0
        var localToGlobalIndex = new NativeArray<int>(sourceMesh.vertexCount, Allocator.Temp);
        var globalToLocalIndex = new NativeHashMap<int, int>(sourceMesh.vertexCount, Allocator.Temp);
        for (int i = 0; i < sourceMesh.vertexCount; ++i)
        {
            localToGlobalIndex[i] = i;
            globalToLocalIndex.TryAdd(i, i);
        }

        // This list holds all constraints in terms of Global indices, but each iteration removes neighbors that have become children 
        var globalNeighborList = CreateNeighborListFromMesh(sourceMesh);
        
        // Keep track of local neighbor counts separately.
        // We want to preserve the global neighbor list for reference, but need to keep track of how connections change as we build the hierarchy
        // NOTE: We store one list for every global index, but the state should always represent our current local state.
        var localCoarseNeighborList = new List<List<int>>(sourceMesh.vertexCount);
        for (int i = 0; i < sourceMesh.vertexCount; ++i)
        {
            localCoarseNeighborList.Add(new List<int>(1));
            localCoarseNeighborList[i].AddRange(globalNeighborList[i]);
        }
        
        var localFineNeighborList = new List<List<int>>(sourceMesh.vertexCount);
        for (int i = 0; i < sourceMesh.vertexCount; ++i)
        {
            localFineNeighborList.Add(new List<int>(1));
        }

        // Do L = 0 ("finest" level w/ all mesh vertices)
        var level0Entity = CreateLevelEntityWithCurrentData(0, sourceMesh, localToGlobalIndex, globalToLocalIndex, globalNeighborList);
        var previousLevel = level0Entity;
        
        // Then iterate! Each iteration starts with the set of coarse vertices from the previous results and tries to further subdivide
        for(int currentHierarchyLevel = 1; currentHierarchyLevel < numberOfHierarchyLevels; ++currentHierarchyLevel)
        {
            var previousLevelVertexCount = localToGlobalIndex.Length;
            var fineIndexCountThisLevel = 0;

            // Parent List per vertex in (currentHierarchyLevel - 1) (previous level)
            // If a vertex becomes "fine", its coarse neighbors are changed to its parents
            var parentGlobalIndexLists = new List<List<int>>(previousLevelVertexCount);
            for (int i = 0; i < previousLevelVertexCount; ++i)
                parentGlobalIndexLists.Add(new List<int>(1));

            // Keep track of which vertices are fine/coarse (clear auto-initializes all to Coarse)
            var localVertexClassifications = new NativeArray<HierarchicalClassification>(previousLevelVertexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
            
            const int kMinimumParentsAllowed = 2;
            for (int localIndexInPreviousLevel = 0; localIndexInPreviousLevel < previousLevelVertexCount; ++localIndexInPreviousLevel)
            {
                var globalIndex = localToGlobalIndex[localIndexInPreviousLevel];
                
                // If my local coarse neighbor count is >= kMinimumParentsAllowed
                if (localCoarseNeighborList[globalIndex].Count >= kMinimumParentsAllowed)
                {
                    // Figure out if all my neighboring *Fine* vertices have a neighbor count greater than kMinimumParentsAllowed
                    var allMyFineNeighborsHaveFineNeighborCountGreaterThanMinimum = true;
                    for (int indexInNeighborList = 0; indexInNeighborList < localFineNeighborList[globalIndex].Count; ++indexInNeighborList)
                    {
                        var neighborGlobalIndex = localFineNeighborList[globalIndex][indexInNeighborList];
                        
                        // skip coarse neighbors
                        if (localVertexClassifications[globalToLocalIndex[neighborGlobalIndex]] == HierarchicalClassification.Coarse)
                            continue;

                        if (localFineNeighborList[neighborGlobalIndex].Count <= kMinimumParentsAllowed)
                        {
                            allMyFineNeighborsHaveFineNeighborCountGreaterThanMinimum = false;
                            break;
                        }
                    }

                    // If the above are both true, I can be marked as "Fine" 
                    if (allMyFineNeighborsHaveFineNeighborCountGreaterThanMinimum)
                    {
                        // Also add my coarse neighbors' global indices to my parent list
                        parentGlobalIndexLists[localIndexInPreviousLevel].AddRange(localCoarseNeighborList[globalIndex]);
                        localVertexClassifications[localIndexInPreviousLevel] = HierarchicalClassification.Fine;
                        fineIndexCountThisLevel++;
                        
                        // Remove me from my neighbors' coarse local neighbor list
                        for (int indexInNeighborList = 0; indexInNeighborList < localCoarseNeighborList[globalIndex].Count; ++indexInNeighborList)
                        {
                            var neighborIndex = localCoarseNeighborList[globalIndex][indexInNeighborList];
                            localCoarseNeighborList[neighborIndex].Remove(globalIndex);
                        }
                        
                        // Clear my coarse local neighbor list
                        localCoarseNeighborList[globalIndex].Clear();
                        
                        // Check if any of my global neighbors are also fine so they can be my neighbors too!
                        var myGlobalNeighborCount = globalNeighborList[globalIndex].Count;
                        for (int indexOfGlobalNeighborIndex = 0; indexOfGlobalNeighborIndex < myGlobalNeighborCount; ++indexOfGlobalNeighborIndex)
                        {
                            var globalNeighborIndex = globalNeighborList[globalIndex][indexOfGlobalNeighborIndex];
                            var globalNeighborIsAlsoLocal = globalToLocalIndex.TryGetValue(globalNeighborIndex, out var localNeighborIndex);
                            
                            // If my neighbor is local...
                            if (globalNeighborIsAlsoLocal)
                            {
                                // and they're fine...
                                if (localVertexClassifications[localNeighborIndex] == HierarchicalClassification.Fine)
                                {
                                    // They can let us know they're fine.
                                    localFineNeighborList[globalIndex].Add(globalNeighborIndex);
                                }
                                else // Otherwise, let our neighbor know that we're fine!
                                {
                                    localFineNeighborList[globalNeighborIndex].Add(globalIndex);
                                }
                            }
                            // (Can't let non-local neighbors know we're fine, they're on vacation and won't be back for a few days)
                        }
                    }
                }
            }
            
            // Ensure at least one constraint exists at this hierarchy level or else we can early out
            var atLeastOneConstraintExists = false;
            for (int i = 0; i < localCoarseNeighborList.Count; ++i)
            {
                var list = localCoarseNeighborList[i];
                if (list.Count > 0)
                {
                    atLeastOneConstraintExists = true;
                    break;
                }
            }

            if (!atLeastOneConstraintExists)
                break;

            // Find all of the "coarse" vertices for our current level and make mappings for global <-> local indices
            var coarseVertexCount = previousLevelVertexCount - fineIndexCountThisLevel;
            var previousLocalToGlobalIndex = localToGlobalIndex;
            var previousGlobalToLocalIndex = globalToLocalIndex;
            
            localToGlobalIndex = new NativeArray<int>(coarseVertexCount, Allocator.Temp);
            globalToLocalIndex = new NativeHashMap<int, int>(coarseVertexCount, Allocator.Temp);

            var counter = 0;
            for (int i = 0; i < previousLevelVertexCount; ++i)
            {
                if (localVertexClassifications[i] == HierarchicalClassification.Coarse)
                {
                    localToGlobalIndex[counter] = previousLocalToGlobalIndex[i];
                    globalToLocalIndex.TryAdd(previousLocalToGlobalIndex[i], counter++);
                }
            }
            
            // Its possible that some of what we decided were parents have now ended up in the coarse group with us, lets fix that
            for (int previousLevelVertexLocalIndex = 0; previousLevelVertexLocalIndex < parentGlobalIndexLists.Count; ++previousLevelVertexLocalIndex)
            {
                var currentVertexParentListCount = parentGlobalIndexLists[previousLevelVertexLocalIndex].Count;
                for (int parentGlobalIndexIndex = 0; parentGlobalIndexIndex < currentVertexParentListCount; ++parentGlobalIndexIndex)
                {
                    if (!globalToLocalIndex.TryGetValue(
                        parentGlobalIndexLists[previousLevelVertexLocalIndex][parentGlobalIndexIndex],
                        out var _))
                    {
                        parentGlobalIndexLists[previousLevelVertexLocalIndex].RemoveAt(parentGlobalIndexIndex);
                        currentVertexParentListCount--;
                        parentGlobalIndexIndex--;
                    }
                }
            }   
            
            // Plus, anything that's still marked coarse should consider itself its own parent
            for (int childLocalIndex = 0; childLocalIndex < previousLevelVertexCount; ++childLocalIndex)
            {
                if (localVertexClassifications[childLocalIndex] == HierarchicalClassification.Coarse)
                {
                    var globalIndex = previousLocalToGlobalIndex[childLocalIndex];
                    parentGlobalIndexLists[childLocalIndex].Add(globalIndex);
                }
            }
            
            // Now that we have all of the parent information for our previous level, we can set their indices and weights
            {
                var parentIndicesAndWeightsBuffer = DstEntityManager.GetBuffer<ClothHierarchicalParentIndexAndWeights>(previousLevel);
                for (int previousLevelVertexLocalIndex = 0; previousLevelVertexLocalIndex < parentGlobalIndexLists.Count; ++previousLevelVertexLocalIndex)
                {
                    var myParentsList = parentGlobalIndexLists[previousLevelVertexLocalIndex];
                    Assert.IsFalse(myParentsList.Count > 16);
                    
                    // initialize all parents and weights to -1 and 0.0f
                    var parentLocalIndexandWeights = parentIndicesAndWeightsBuffer[previousLevelVertexLocalIndex];
                    for (int i = 0; i < 16; ++i)
                    {
                        parentLocalIndexandWeights.ParentIndex[i] = -1;
                        parentLocalIndexandWeights.WeightValue[i] = 0.0f;
                    }
                    parentLocalIndexandWeights.ParentCount = myParentsList.Count;
                    
                    // Store indices and calculate max distance of all parents
                    var myPosition = sourceMesh.vertices[previousLocalToGlobalIndex[previousLevelVertexLocalIndex]];
                    var maxParentDistance = 0.0f;
                    for (int parentGlobalIndexIndex = 0; parentGlobalIndexIndex < myParentsList.Count; ++parentGlobalIndexIndex)
                    {
                        var parentGlobalIndex = myParentsList[parentGlobalIndexIndex];
                        parentLocalIndexandWeights.ParentIndex[parentGlobalIndexIndex] = globalToLocalIndex[parentGlobalIndex];
                        
                        var parentPosition = sourceMesh.vertices[myParentsList[parentGlobalIndexIndex]];
                        maxParentDistance = math.max(maxParentDistance, math.distance(myPosition, parentPosition));
                    }

                    // Calculate weights
                    var totalWeights = 0.0f;
                    for (int parentGlobalIndexIndex = 0; parentGlobalIndexIndex < myParentsList.Count; ++parentGlobalIndexIndex)
                    {
                        var parentPosition = sourceMesh.vertices[myParentsList[parentGlobalIndexIndex]];
                        var weight = 1.0f / 
                                     (math.distance(myPosition, parentPosition) / (maxParentDistance + 0.001f) + 0.001f); // todo: epsilon to prevent divide by zero?
                        parentLocalIndexandWeights.WeightValue[parentGlobalIndexIndex] = weight;
                        totalWeights += weight;
                    }
                    
                    // Normalize weights
                    for (int parentGlobalIndexIndex = 0; parentGlobalIndexIndex < myParentsList.Count; ++parentGlobalIndexIndex)
                    {
                        float originalWeight = parentLocalIndexandWeights.WeightValue[parentGlobalIndexIndex];
                        parentLocalIndexandWeights.WeightValue[parentGlobalIndexIndex] = originalWeight / totalWeights;
                    }
                    
                    // Set value back to the buffer
                    parentIndicesAndWeightsBuffer[previousLevelVertexLocalIndex] = parentLocalIndexandWeights;
                }
            }
            
            // Don't need parent information anymore, can scrap it!
            previousLocalToGlobalIndex.Dispose();
            previousGlobalToLocalIndex.Dispose();

            // Create the entity for this hierarchy level
            var currentLevelEntity = CreateLevelEntityWithCurrentData(currentHierarchyLevel, sourceMesh, localToGlobalIndex, globalToLocalIndex, localCoarseNeighborList);
            
            // Set this level as the previous' parent
            DstEntityManager.SetComponentData(previousLevel, new ClothHierarchyParentEntity{Parent = currentLevelEntity});
            previousLevel = currentLevelEntity;
            
            // Clear fine neighbor lists (all vertices are marked "coarse" each iteration!)
            for (int i = 0; i < sourceMesh.vertexCount; ++i)
            {
                localFineNeighborList[i].Clear();
            }
        }
        
        // Delete the parent info from the top level (don't have parents)
        DstEntityManager.RemoveComponent<ClothHierarchyParentEntity>(previousLevel);
        DstEntityManager.RemoveComponent<ClothHierarchicalParentIndexAndWeights>(previousLevel);

        return level0Entity;
    }
    
    private void InitializeHierarchical(Entity renderingEntity, Transform transform, MeshFilter meshFilter, VertexClothGarment garment)
    {
        var mesh = meshFilter.mesh;
        
        // Add reference to source mesh data and set it as read/write
        mesh.MarkDynamic();

        var meshHandle = GCHandle.Alloc(mesh, GCHandleType.Pinned);
        
        // Setup constraint hierarchy
        const int kNumHiearchyLevels = 3;
        var level0Entity = CreateHierarchy(kNumHiearchyLevels, mesh);

        // Set sourcemeshdata on the l=0 entity so we can write back positions later
        var srcMeshData = new ClothSourceMeshData
        {
            SrcMeshHandle = meshHandle
        };
        DstEntityManager.AddComponentData(level0Entity, srcMeshData);
    }
    
    private void InitializeNonHierarchical(Transform transform, MeshFilter meshFilter, VertexClothGarment garment)
    {
        var mesh = meshFilter.mesh;
        var vertexCount = mesh.vertexCount;
        
        var archetype = DstEntityManager.CreateArchetype( 
            typeof(ClothProjectedPosition), 
            typeof(ClothCurrentPosition), 
            typeof(ClothPreviousPosition), 
            typeof(ClothDistanceConstraint), 
            typeof(ClothPositionOrigin), 
            typeof(ClothPinWeight),
            typeof(ClothTotalTime),
            typeof(ClothTimestepData),
            typeof(ClothSourceMeshData),
            typeof(ClothSphereCollider),
            typeof(ClothCapsuleCollider),
            typeof(ClothPlaneCollider),
            typeof(ClothCollisionContact),
            typeof(ClothWorldToLocal));
        var entity = DstEntityManager.CreateEntity(archetype);
        
        // Add reference to source mesh data and set it as read/write
        mesh.MarkDynamic();

        var meshHandle = GCHandle.Alloc(mesh, GCHandleType.Pinned);
        var srcMeshData = new ClothSourceMeshData
        {
            SrcMeshHandle = meshHandle
        };
        DstEntityManager.SetComponentData(entity, srcMeshData);
        DstEntityManager.SetComponentData(entity, new ClothTotalTime { TotalTime = 0.0f });
        DstEntityManager.SetComponentData(entity, new ClothTimestepData {FixedTimestep = 1.0f / 60.0f, IterationCount = 0});
        DstEntityManager.SetComponentData(entity, new ClothWorldToLocal { Value = transform.worldToLocalMatrix });

        // Copy initial vert data to buffer
        var projectedPositionBuffer = DstEntityManager.GetBuffer<ClothProjectedPosition>(entity);
        projectedPositionBuffer.Reserve(vertexCount);
        
        var currentPositionBuffer = DstEntityManager.GetBuffer<ClothCurrentPosition>(entity);
        currentPositionBuffer.Reserve(vertexCount);
        
        var previousPositionBuffer = DstEntityManager.GetBuffer<ClothPreviousPosition>(entity);
        previousPositionBuffer.Reserve(vertexCount);
        
        var originPositionBuffer = DstEntityManager.GetBuffer<ClothPositionOrigin>(entity);
        originPositionBuffer.Reserve(vertexCount);

        fixed (Vector3* positions = mesh.vertices)
        {
            var currentPositionsAsNativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ClothCurrentPosition>((float3*) positions, vertexCount, Allocator.Invalid);
            var projectedPositionsAsNativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ClothProjectedPosition>((float3*) positions, vertexCount, Allocator.Invalid);
            var previousPositionsAsNativeArray  = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ClothPreviousPosition>((float3*) positions, vertexCount, Allocator.Invalid);
            var originPositionsAsNativeArray  = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ClothPositionOrigin>((float3*) positions, vertexCount, Allocator.Invalid);
            
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref currentPositionsAsNativeArray, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref projectedPositionsAsNativeArray, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref previousPositionsAsNativeArray, AtomicSafetyHandle.Create());
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref originPositionsAsNativeArray, AtomicSafetyHandle.Create());
#endif
            currentPositionBuffer.CopyFrom(currentPositionsAsNativeArray);
            projectedPositionBuffer.CopyFrom(projectedPositionsAsNativeArray);
            previousPositionBuffer.CopyFrom(previousPositionsAsNativeArray);
            originPositionBuffer.CopyFrom(originPositionsAsNativeArray);
        }
            
        // Add constraints to the entity
        // todo: no garbage
        var barLookup = new HashSet<Vector2Int>();
        var triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2Int pair = new Vector2Int
                {
                    x = triangles[i + j],
                    y = triangles[i + (j + 1) % 3]
                };
                
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
        
        // todo: no garbage
        var barList = new List<Vector2Int>(barLookup);
        var constraintCount = barList.Count;
        
        var constraintsBuffer = DstEntityManager.GetBuffer<ClothDistanceConstraint>(entity);
        constraintsBuffer.Reserve(constraintCount);

        var vertices = mesh.vertices;
        for (int i = 0; i < constraintCount; ++i)
        {
            Vector3 p1 = vertices[barList[i].x];
            Vector3 p2 = vertices[barList[i].y];
            
            constraintsBuffer.Add(new ClothDistanceConstraint
            {
                RestLengthSqr = (p2 - p1).sqrMagnitude,
                VertexA = barList[i].x,
                VertexB = barList[i].y
            });
        }
        
        // Add pin weights
        var pinWeightBuffer = DstEntityManager.GetBuffer<ClothPinWeight>(entity);
        pinWeightBuffer.Reserve(vertexCount);

        var normals = mesh.normals;
        for (int i = 0; i < vertexCount; ++i)
        {
            //if (normals[i].y > .9f && vertices[i].y > .3f)
            if (math.abs(vertices[i].x) > 2.25f)
                pinWeightBuffer.Add(new ClothPinWeight {InvPinWeight = 0.0f});
            else
                pinWeightBuffer.Add(new ClothPinWeight {InvPinWeight = 1.0f});
        }
        
        // Sphere Colliders
        var sphereColliders = DstEntityManager.GetBuffer<ClothSphereCollider>(entity);
        sphereColliders.Reserve(garment.m_SphereColliders.Length);
        for (int i = 0; i < garment.m_SphereColliders.Length; ++i)
        {
            var sphere = garment.m_SphereColliders[i];
            sphereColliders.Add(new ClothSphereCollider{LocalCenter = sphere.xyz, Radius = sphere.w});
        }
        
        // todo Capsule Colliders
        //var capsuleColliders = DstEntityManager.GetBuffer<ClothCapsuleCollider>(entity);
        //capsuleColliders.Reserve(garment.m_CapsuleColliders.Length);
        //for (int i = 0; i < garment.m_CapsuleColliders.Length; ++i)
        //{
        //    var capsule = garment.m_CapsuleColliders[i];
        //    var vertA = capsule.center + ((capsule.height - capsule.radius) * capsule.direction);
        //    
        //    capsuleColliders.Add(new ClothCapsuleCollider{LocalCenter = capsule., Radius = collider.radius});
        //}
        
        // Plane Colliders
        var planeColliders = DstEntityManager.GetBuffer<ClothPlaneCollider>(entity);
        planeColliders.Reserve(garment.m_PlaneColliders.Length);
        for (int i = 0; i < garment.m_PlaneColliders.Length; ++i)
        {
            var plane = garment.m_PlaneColliders[i];
            planeColliders.Add(new ClothPlaneCollider{LocalNormal = plane.xyz, LocalOffset = plane.w});
        }
    }
    
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity renderingEntity, Transform transform, MeshFilter meshFilter, VertexClothGarment garment) =>
        {
            if (garment.isHierarchical)
            {
                // Abandon hope all ye who enter here...
                InitializeHierarchical(renderingEntity, transform, meshFilter, garment);
            }
            else
            {
                InitializeNonHierarchical(transform, meshFilter, garment);
            }
        });
    }
}