using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TrainPositioningSystem : JobComponentSystem
{
    public NativeArray<float3> m_PathPositions;
    public BitArray m_PathStopBits;
    public NativeArray<int2> m_StartEndPositionIndicies;
    public int m_PathCount;
    public int m_SinglePathStartIndex;
    
    EntityQuery m_Query;
    List<TrackIndexComponent> m_UniqueTracks;
    NativeArray<JobHandle> m_Handles;

    protected override void OnCreate()
    {
        // Cached access to a set of ComponentData based on a specific query
        m_Query = GetEntityQuery(typeof(Translation),  typeof(Rotation), ComponentType.ReadOnly<CarriageComponent>(), ComponentType.ReadOnly<TrackIndexComponent>());
        RequireForUpdate(m_Query);
    }


    // This declares a new kind of job, which is a unit of work to do.
    // The job is declared as an IJobForEach<Translation, Rotation>,
    // meaning it will process all entities in the world that have both
    // Translation and Rotation components. Change it to process the component
    // types you want.
    //
    // The job is also tagged with the BurstCompile attribute, which means
    // that the Burst compiler will optimize it for the best performance.
    [BurstCompile]
    struct TrainPositioningSystemJob : IJobChunk
    {
        public float m_DeltaTime;
        public float m_MaxSpeed;
        [NativeDisableContainerSafetyRestriction]
        public ArchetypeChunkComponentType<Translation> m_TranslationType;
        [NativeDisableContainerSafetyRestriction]
        public ArchetypeChunkComponentType<Rotation> m_RotationType;
        [NativeDisableContainerSafetyRestriction]
        public ArchetypeChunkComponentType<CarriageComponent> m_CarriageComponent;

        [ReadOnly] public NativeArray<float3> m_PathPositions;
        [ReadOnly] public BitArray m_PathStopBits;
        [ReadOnly] public int2 m_StartEndPositionIndicies;

        
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Translation> chunkTranslations = chunk.GetNativeArray(m_TranslationType);
            NativeArray<Rotation> chunkRotations = chunk.GetNativeArray(m_RotationType);
            NativeArray<CarriageComponent> chunkCarriages = chunk.GetNativeArray(m_CarriageComponent);

            for (int i = 0; i < chunk.Count; i++)
            {
                Rotation rotation = chunkRotations[i];
                Translation translation = chunkTranslations[i];
                CarriageComponent carriageComponent = chunkCarriages[i];                
                int currPointIndex = carriageComponent.m_CurrentPointIndex;
                int nextPointIndex = m_StartEndPositionIndicies.x + (currPointIndex - m_StartEndPositionIndicies.x + 1) % (m_StartEndPositionIndicies.y - m_StartEndPositionIndicies.x);
                float3 currPosition = translation.Value;
                float3 nextPosition = m_PathPositions[nextPointIndex];
                float3 vecToNextPosition = nextPosition - currPosition;
                float distToNextPosition = math.length(vecToNextPosition);
                float distInUpdate = (m_MaxSpeed * m_DeltaTime);
                if(distInUpdate >= distToNextPosition)
                {
                    currPosition = nextPosition;
                    currPointIndex = nextPointIndex;
                    nextPointIndex = m_StartEndPositionIndicies.x + (currPointIndex - m_StartEndPositionIndicies.x + 1) % (m_StartEndPositionIndicies.y - m_StartEndPositionIndicies.x);
                    nextPosition = m_PathPositions[nextPointIndex];
                    distInUpdate -= distToNextPosition;

                    vecToNextPosition = nextPosition - currPosition;
                    distToNextPosition = math.length(vecToNextPosition);
                }
                float3 forward = vecToNextPosition / distToNextPosition;
                carriageComponent.m_CurrentPointIndex = currPointIndex;
                translation.Value = currPosition + forward * distInUpdate;
                rotation.Value = quaternion.LookRotation(forward, new float3(0,1,0));
                chunkRotations[i] = rotation;
                chunkTranslations[i] = translation;
                chunkCarriages[i] = carriageComponent;       
            }
        }                
    }

    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        if(!m_Handles.IsCreated)
        {
            m_Handles = new NativeArray<JobHandle>(m_SinglePathStartIndex, Allocator.Persistent);
        }

        if(m_UniqueTracks == null)
        {
            m_UniqueTracks = new List<TrackIndexComponent>(m_SinglePathStartIndex);
        }
        
        EntityManager.GetAllUniqueSharedComponentData(m_UniqueTracks);
        for(int trackIndex = 0; trackIndex < m_UniqueTracks.Count; trackIndex++)        
        {
            m_Query.AddSharedComponentFilter(m_UniqueTracks[trackIndex]);
            int entityCount = m_Query.CalculateEntityCount();
            TrainPositioningSystemJob job = new TrainPositioningSystemJob();        
            job.m_DeltaTime = Time.DeltaTime;
            job.m_MaxSpeed = Metro.INSTANCE().maxTrainSpeed[m_UniqueTracks[trackIndex].m_TrackIndex];
            job.m_TranslationType = GetArchetypeChunkComponentType<Translation>();
            job.m_RotationType = GetArchetypeChunkComponentType<Rotation>();
            job.m_CarriageComponent = GetArchetypeChunkComponentType<CarriageComponent>();
            job.m_PathPositions = m_PathPositions;
            job.m_PathStopBits = m_PathStopBits;
            job.m_StartEndPositionIndicies = m_StartEndPositionIndicies[m_UniqueTracks[trackIndex].m_TrackIndex];
            m_Handles[trackIndex] = job.Schedule(m_Query, inputDependencies);   
            //job.Run(m_Query);
            m_Query.ResetFilter();
        }     
        m_UniqueTracks.Clear();
        JobHandle outputDependencies = JobHandle.CombineDependencies(m_Handles);
        outputDependencies.Complete();
        return outputDependencies;
    }

    protected override void OnDestroy()
    {
        if(m_PathPositions.IsCreated)
        {
            m_PathPositions.Dispose();
        }

        if(m_PathStopBits.IsCreated)
        {
            m_PathStopBits.Dispose();
        }

        if(m_StartEndPositionIndicies.IsCreated)
        {
            m_StartEndPositionIndicies.Dispose();
        }

        if(m_Handles.IsCreated)
        {
            m_Handles.Dispose();
        }        
    }
}