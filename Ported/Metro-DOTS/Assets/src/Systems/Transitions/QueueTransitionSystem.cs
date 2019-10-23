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

        var copyJob = new CopyLoadingStationIDs
        {
            outputBuffer = loadingStationIDs
        };

        var copyJobHandle = copyJob.Schedule(m_LoadingStationsQuery, inputDeps);











//        var job = new ApplyQueueTransition
//        {
//            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
//        };

        var handle = job.Schedule(this, inputDeps);
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

//    struct ApplyQueueTransition : IJobForEach<>
}
