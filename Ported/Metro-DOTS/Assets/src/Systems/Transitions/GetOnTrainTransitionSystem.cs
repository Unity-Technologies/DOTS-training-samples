using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class GetOnTrainTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    EntityQuery m_loadingQuery;
    EntityQuery m_GettingOnQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_loadingQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<LOADING>(),
                    ComponentType.ReadOnly<TrainId>()
                }
            });

        m_GettingOnQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<GET_ON_TRAIN>(),
                    ComponentType.ReadOnly<TrainId>(),
                    ComponentType.ReadOnly<DistanceToTarget>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var loadingEntitiesCount = m_loadingQuery.CalculateEntityCount();
        var loadingTrainIDs = new NativeArray<int>(loadingEntitiesCount, Allocator.TempJob);

        // Step 1. Get all the indices of entities that are tagged with LOADING
        var copyJob = new CopyClosingDoorsIDs
        {
            outputBuffer = loadingTrainIDs
        };

        var copyJobHandle = copyJob.Schedule(m_loadingQuery, inputDeps);

        // Step 2. Compare every entity tagged with QUEUE on their PlatformId with all the
        // stored entities that are LOADING.
        var job = new ApplyGetOnTrainTransition
        {
            inputBuffer = loadingTrainIDs,
            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var handle = job.Schedule(m_GettingOnQuery, copyJobHandle);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    [BurstCompile]
    struct CopyClosingDoorsIDs : IJobForEachWithEntity<TrainId>
    {
        [WriteOnly]
        public NativeArray<int> outputBuffer;

        public void Execute(Entity entity, int jobIndex, ref TrainId trainId)
        {
            outputBuffer[jobIndex] = trainId.value;
        }
    }

    struct ApplyGetOnTrainTransition : IJobForEachWithEntity<TrainId, DistanceToTarget>
    {
        [DeallocateOnJobCompletion]
        public NativeArray<int> inputBuffer;
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex, [ReadOnly] ref TrainId trainId, ref DistanceToTarget distanceToSeat)
        {
            if (distanceToSeat.value > 1)
                return;

            var found = false;
            for (int i = 0; i < inputBuffer.Length; ++i)
                found = found || inputBuffer[i] == trainId.value;

            if (!found)
                return;

            commandBuffer.RemoveComponent<GET_ON_TRAIN>(jobIndex, entity);
            commandBuffer.AddComponent<WAIT_FOR_STOP>(jobIndex, entity);
        }
    }
}
