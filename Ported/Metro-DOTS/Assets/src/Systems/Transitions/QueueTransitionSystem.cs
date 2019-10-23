using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class QueueTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    EntityQuery m_LoadingStationsQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_LoadingStationsQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<LOADING>(),
                    ComponentType.ReadOnly<PlatformId>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var loadingStationIDsCount = m_LoadingStationsQuery.CalculateEntityCount();
        var loadingStationIDs = new NativeArray<uint>(loadingStationIDsCount, Allocator.TempJob);

        // Step 1. Get all the indices of entities that are tagged with LOADING
        var copyJob = new CopyLoadingStationIDs
        {
            outputBuffer = loadingStationIDs
        };

        var copyJobHandle = copyJob.Schedule(m_LoadingStationsQuery, inputDeps);

        // Step 2. Compare every entity tagged with QUEUE on their PlatformId with all the
        // stored entities that are LOADING.
        var job = new ApplyQueueTransition
        {
            inputBuffer = loadingStationIDs,
            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var handle = job.Schedule(this, copyJobHandle);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }


    [BurstCompile]
    struct CopyLoadingStationIDs : IJobForEachWithEntity<PlatformId>
    {
        [WriteOnly]
        public NativeArray<uint> outputBuffer;

        public void Execute(Entity entity, int jobIndex, ref PlatformId platformID)
        {
            outputBuffer[jobIndex] = platformID.value;
        }
    }

    [RequireComponentTag(typeof(QUEUE))]
    struct ApplyQueueTransition : IJobForEachWithEntity<PlatformId>
    {
        [DeallocateOnJobCompletion]
        public NativeArray<uint> inputBuffer;
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex, ref PlatformId platformID)
        {
            var found = false;
            for (int i = 0; i < inputBuffer.Length; ++i)
                found = found || inputBuffer[i] == platformID.value;

            if (found)
            {
                commandBuffer.RemoveComponent<QUEUE>(jobIndex, entity);
                commandBuffer.AddComponent<GET_ON_TRAIN>(jobIndex, entity);
            }
        }
    }
}
