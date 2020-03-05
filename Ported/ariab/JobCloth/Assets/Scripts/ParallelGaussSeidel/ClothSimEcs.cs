using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct ClothGarment : ISharedComponentData
{
    public byte GarmentIndex;
}

public struct ClothGarmentIndex : IComponentData
{
    public byte GarmentIndex;
}

public struct PreviousPosition : IComponentData
{
    public float3 Value;
}

public struct ProjectedPosition : IComponentData
{
    public float3 Value;
}

// Cannot be a tag since we must maintain chunk ordering of vertices
// 0 if unpinned 1 if pinned1
public struct PinWeight : IComponentData
{
    public byte Value;
}

public struct DistanceConstraint : IComponentData
{
    public Entity A;
    public Entity B;
    public float RestLengthSqr;
}

public struct ConstraintCluster : ISharedComponentData
{
    public int ClusterIndex;
}

public class ClothSimSystem : JobComponentSystem
{
    #region Static Things
    
    // needs to be a managed array since Mesh is managed data
    private static Mesh[] s_allGarmentMeshes;
    private static NativeArray<float4x4> s_allGarmentWorldToLocals;
    private static int s_allGarmentCount;

    public static void SetStaticGarmentListCapacity(int newCapacity)
    {
        if (s_allGarmentWorldToLocals.IsCreated)
        {
            throw new InvalidOperationException("Looks like we're resetting the static list capacity. This is a bug.");
        }

        // First garment is null garment
        // This matches behavior of SharedComponentData
        newCapacity += 1;

        s_allGarmentWorldToLocals = new NativeArray<float4x4>(newCapacity, Allocator.Persistent);
        s_allGarmentMeshes = new Mesh[newCapacity];
        s_allGarmentMeshes[0] = null;
        s_allGarmentCount = 1;
    }

    public static void AddGarmentDataToStaticList(Mesh mesh, float4x4 worldToLocal)
    {
        s_allGarmentWorldToLocals[s_allGarmentCount] = worldToLocal;
        s_allGarmentMeshes[s_allGarmentCount++] = mesh;
    }
    
    #endregion
    
    private EntityQuery m_allClothPointsGroup;
    private EntityQuery m_clothConstraintsGroup;

    [BurstCompile]
    struct CopySimPointsToVerticesJob : IJobForEachWithEntity<Translation>
    {
        public NativeArray<Vector3> Vertices;
        
        public void Execute(Entity entity, int index, [ReadOnly]ref Translation position)
        {
            var currentPosition = position.Value;

            Vertices[index] = new Vector3
            {
                x = currentPosition.x,
                y = currentPosition.y,
                z = currentPosition.z
            };
        }
    }

    [BurstCompile]
    private struct ProjectPositionJob : IJobForEach<Translation, PreviousPosition, ProjectedPosition, PinWeight, ClothGarmentIndex>
    {
        public float Dt;
        public float4 GravityDt;

        [ReadOnly]
        public NativeArray<float4x4> WorldToLocals;
               
        public void Execute(
            [ReadOnly]ref Translation current, 
            [ReadOnly]ref PreviousPosition previous, 
            ref ProjectedPosition projected, 
            [ReadOnly]ref PinWeight pin,
            [ReadOnly]ref ClothGarmentIndex index)
        {
            // todo: this makes me sad
            if (pin.Value == 1)
                return;

            var verletVelocity = (current.Value - previous.Value);
            verletVelocity += math.mul(WorldToLocals[index.GarmentIndex], GravityDt * Dt).xyz;
            
            var projectedPosition = current.Value + verletVelocity;
            projected = new ProjectedPosition
            {
                Value = projectedPosition
            };
        }
    }

    [BurstCompile]
    private struct ConstraintSolverJob : IJobForEach<DistanceConstraint>
    {
        [NativeDisableContainerSafetyRestriction]
        public ComponentDataFromEntity<ProjectedPosition> Positions;

        [ReadOnly]        
        public ComponentDataFromEntity<PinWeight> Pins;

        public void Execute([ReadOnly]ref DistanceConstraint constraint)
        {
            // todo: Accessing component data by entity is slow. Is there a better way?
            // copy positions to an array and use indices?
            // this is the part where using ECS is worse than just using native arrays
            var positionA = Positions[constraint.A];
            var positionB = Positions[constraint.B];
            var restLengthSqr = constraint.RestLengthSqr;
            
            float length = math.length(positionB.Value - positionA.Value);
            float extra = (length - math.sqrt(restLengthSqr)) * 0.5f;
            float3 dir = math.normalize(positionB.Value - positionA.Value);
            
            if(Pins[constraint.A].Value == 0)
                Positions[constraint.A] = new ProjectedPosition {Value = positionA.Value + (extra * dir)};
            if(Pins[constraint.B].Value == 0)
                Positions[constraint.B] = new ProjectedPosition {Value = positionB.Value - (extra * dir)};
        }
    }

    [BurstCompile]
    private struct CopyPositionsJob : IJobForEach<Translation, PreviousPosition, ProjectedPosition>
    {
        public void Execute(ref Translation current, ref PreviousPosition previous, [ReadOnly]ref ProjectedPosition projected)
        {
            previous = new PreviousPosition {Value = current.Value};
            current = new Translation {Value = projected.Value};
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {        
        m_allClothPointsGroup.ResetFilter();
        
        // todo: preallocate this, garbage is gross
        var garmentList = new List<ClothGarment>();
        EntityManager.GetAllUniqueSharedComponentData(garmentList);
        var garmentCount = garmentList.Count;
                
        // First project positions for all points        
        var dt = 1.0f / 60.0f;
        var gravity = new float4(0.0f, -9.8f * dt, 0.0f, 0.0f);
        var projectJob = new ProjectPositionJob
        {
            Dt = dt,
            GravityDt = gravity,
            
            WorldToLocals = s_allGarmentWorldToLocals
        };
        var projectHandle = projectJob.Schedule(m_allClothPointsGroup, inputDeps);
        
        // foreach cluster run solver iterations
        var clusterList = new List<ConstraintCluster>();
        EntityManager.GetAllUniqueSharedComponentData(clusterList);

        var clusterCount = clusterList.Count;
        var allPreviousSolversHandle = projectHandle;

        const int kIterationCount = 8;
        for (int iterationIndex = 0; iterationIndex < kIterationCount; ++iterationIndex)
        {
            for (int clusterIndex = 1; clusterIndex < clusterCount; ++clusterIndex)
            {
                m_clothConstraintsGroup.SetSharedComponentFilter(new ConstraintCluster {ClusterIndex = clusterIndex});
                var solverJob = new ConstraintSolverJob
                {
                    Positions = GetComponentDataFromEntity<ProjectedPosition>(),
                    Pins = GetComponentDataFromEntity<PinWeight>()
                };
                var solverHandle = solverJob.Schedule(m_clothConstraintsGroup, allPreviousSolversHandle);
                allPreviousSolversHandle = JobHandle.CombineDependencies(allPreviousSolversHandle, solverHandle);
            }
        }

        var copyPositionsJob = new CopyPositionsJob();
        var copyHandle = copyPositionsJob.Schedule(m_allClothPointsGroup, allPreviousSolversHandle);
        
        // todo: Mesh update currently requires syncing the jobs and causes a stall.
        // better way to do this: copy out the results from the previous frame at the beginning of each frame
        // then run mesh update on main thread at the same time as the sim for the current frame
        
        #region update mesh
        
        copyHandle.Complete();
       
        var vertexPositions = new List<NativeArray<Vector3>>(garmentCount - 1);
        
        // Write last frame's results to mesh for each garment
        // Slowest part of the process and unfortunately must be done on main thread until we have unmanaged mesh type
        var copyAllVertexPositionsHandle = inputDeps;
        for (int i = 1; i < garmentCount; ++i)
        {
            var garment = garmentList[i];
            var garmentMesh = s_allGarmentMeshes[garment.GarmentIndex];
            var vertexCount = garmentMesh.vertexCount;

            var simPointsAsVertices = new NativeArray<Vector3>(vertexCount, Allocator.TempJob);
            vertexPositions.Add(simPointsAsVertices);

            m_allClothPointsGroup.SetSharedComponentFilter(garment);
            var copyToVertexJob = new CopySimPointsToVerticesJob
            {
                Vertices = simPointsAsVertices
            };
            var copyToVertexHandle = copyToVertexJob.Schedule(m_allClothPointsGroup, inputDeps);
            copyAllVertexPositionsHandle = JobHandle.CombineDependencies(copyAllVertexPositionsHandle, copyToVertexHandle);
        }
        
        // todo: Gross sync point, can we get rid of this?
        // we probably can since all we really need is a copy of the data
        // then we can do the write back to mesh on the main thread while sim runs
        copyAllVertexPositionsHandle.Complete();
        
        // Write back to mesh (slow and single threaded by necessity :(
        for (int i = 1; i < garmentCount; ++i)
        {
            var garment = garmentList[i];
            var garmentMesh = s_allGarmentMeshes[garment.GarmentIndex];     
            var verticesNative = vertexPositions[i - 1];
            var newVertexArray = new Vector3[verticesNative.Length]; // todo: preallocate this garbage is gross
            verticesNative.CopyTo(newVertexArray);

            garmentMesh.vertices = newVertexArray;
            
            verticesNative.Dispose();
        }
        #endregion
        
        return allPreviousSolversHandle;
    }

    protected override void OnCreate()
    {   
        m_allClothPointsGroup = GetEntityQuery(
            typeof(Translation), 
            typeof(PreviousPosition), 
            typeof(ProjectedPosition), 
            typeof(PinWeight),
            typeof(ClothGarmentIndex),
            typeof(ClothGarment));

        m_clothConstraintsGroup = GetEntityQuery(
            typeof(DistanceConstraint),
            typeof(ConstraintCluster));
    }

    protected override void OnDestroy()
    {
        if(s_allGarmentWorldToLocals.IsCreated)
            s_allGarmentWorldToLocals.Dispose();
    }
}
