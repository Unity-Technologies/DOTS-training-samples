using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class QueueTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    EntityQuery m_LoadingStationsQuery;
    EntityQuery m_QueueingQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_LoadingStationsQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<LOADING>(),
                    ComponentType.ReadOnly<PlatformId>(),
                    ComponentType.ReadOnly<TrainId>(),
                    ComponentType.ReadOnly<Translation>()
                }
            });;

        m_QueueingQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<QUEUE>(),
                    ComponentType.ReadOnly<PlatformId>(),
                    ComponentType.ReadWrite<TrainId>(),
                    ComponentType.ReadWrite<TargetPosition>()
                }
            });
    }

    struct TrainToPlatformPosition
    {
        public int trainID;
        public int platformID;
        public float3 trainPosition;

        public TrainToPlatformPosition(int trainID, int platformID, float3 trainPosition)
        {
            this.trainID = trainID;
            this.platformID = platformID;
            this.trainPosition = trainPosition;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var loadingStationIDsCount = m_LoadingStationsQuery.CalculateEntityCount();
        var loadingTrains = new NativeArray<TrainToPlatformPosition>(loadingStationIDsCount, Allocator.TempJob);

        // Step 1. Get all the indices of entities that are tagged with LOADING
        var copyJob = new CopyLoadingStationIDs
        {
            outputBuffer = loadingTrains
        };

        var copyJobHandle = copyJob.Schedule(m_LoadingStationsQuery, inputDeps);

        // Step 2. Compare every entity tagged with QUEUE on their PlatformId with all the
        // stored entities that are LOADING.
        var job = new ApplyQueueTransition
        {
            inputBuffer = loadingTrains,
            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var handle = job.Schedule(m_QueueingQuery, copyJobHandle);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }


    [BurstCompile]
    struct CopyLoadingStationIDs : IJobForEachWithEntity<PlatformId, TrainId, TargetPosition>
    {
        [WriteOnly]
        public NativeArray<TrainToPlatformPosition> outputBuffer;
        public void Execute(Entity entity, int jobIndex, ref PlatformId platformID, ref TrainId trainId, ref TargetPosition trainPosition)
        {
            outputBuffer[jobIndex] = new TrainToPlatformPosition(trainId.value, platformID.value, trainPosition.value);
        }
    }

    struct ApplyQueueTransition : IJobForEachWithEntity<PlatformId, TrainId, TargetPosition>
    {
        [DeallocateOnJobCompletion]
        public NativeArray<TrainToPlatformPosition> inputBuffer;
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex, [ReadOnly] ref PlatformId platformID, ref TrainId trainId, ref TargetPosition seatPosition)
        {
            int trainIndex = -1;
            for (int i = 0; i < inputBuffer.Length; ++i)
            {
                if(inputBuffer[i].platformID == platformID.value)
                {
                    trainIndex = i;
                }
            }
            if (trainIndex < 0)
                return;

            var ableToGetOn = true; // TODO: Implement
            if (!ableToGetOn)
                return;

            trainId.value = inputBuffer[trainIndex].trainID;
            seatPosition.value = inputBuffer[trainIndex].trainPosition; // Currently assign TargetPosition to train position
            commandBuffer.RemoveComponent<QUEUE>(jobIndex, entity);
            commandBuffer.AddComponent<GET_ON_TRAIN>(jobIndex, entity);
        }
    }
}
