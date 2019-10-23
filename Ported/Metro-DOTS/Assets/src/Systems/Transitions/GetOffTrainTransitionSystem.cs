using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class GetOffTrainTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    EntityQuery m_LoadingQuery;
    EntityQuery m_GettingOffQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_LoadingQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<LOADING>(),
                    ComponentType.ReadOnly<TrainId>()
                }
            });

        m_GettingOffQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<GET_OFF_TRAIN>(),
                    ComponentType.ReadOnly<TrainId>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var loadingEntitiesCount = m_LoadingQuery.CalculateEntityCount();
        var loadingIDs = new NativeArray<uint>(loadingEntitiesCount, Allocator.TempJob);

        // Step 1. Get all the indices of entities that are tagged with LOADING
        var copyJob = new CopyLoadingIDs
        {
            outputBuffer = loadingIDs
        };

        var copyJobHandle = copyJob.Schedule(m_LoadingQuery, inputDeps);

        // Step 2. Compare every entity tagged with QUEUE on their PlatformId with all the
        // stored entities that are LOADING.
        var job = new ApplyGetOffTrainTransition
        {
            inputBuffer = loadingIDs,
            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var handle = job.Schedule(m_GettingOffQuery, copyJobHandle);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    [BurstCompile]
    struct CopyLoadingIDs : IJobForEachWithEntity<TrainId>
    {
        [WriteOnly]
        public NativeArray<uint> outputBuffer;

        public void Execute(Entity entity, int jobIndex, ref TrainId trainId)
        {
            outputBuffer[jobIndex] = trainId.value;
        }
    }

    struct ApplyGetOffTrainTransition : IJobForEachWithEntity<TrainId>
    {
        [DeallocateOnJobCompletion]
        public NativeArray<uint> inputBuffer;
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex, [ReadOnly] ref TrainId trainId)
        {
            var found = false;
            for (int i = 0; i < inputBuffer.Length; ++i)
                found = found || inputBuffer[i] == trainId.value;

            if (!found)
                return;

            commandBuffer.RemoveComponent<GET_OFF_TRAIN>(jobIndex, entity);
            commandBuffer.AddComponent<WALK>(jobIndex, entity);
        }
    }
}
