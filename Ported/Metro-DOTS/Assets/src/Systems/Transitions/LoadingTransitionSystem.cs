using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class LoadingTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    EntityQuery m_LoadingQuery;
    EntityQuery m_GettingOnQuery;

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

        m_GettingOnQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<GET_ON_TRAIN>(),
                    ComponentType.ReadOnly<TrainId>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gettingOnEntitiesCount = m_GettingOnQuery.CalculateEntityCount();
        var gettingOnIDs = new NativeArray<int>(gettingOnEntitiesCount, Allocator.TempJob);

        var copyJob = new CopyGettingOnIDs
        {
            outputBuffer = gettingOnIDs
        };

        var copyJobHandle = copyJob.Schedule(m_GettingOnQuery, inputDeps);

        var job = new ApplyLoadingTrainTransition
        {
            inputBuffer = gettingOnIDs,
            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var handle = job.Schedule(m_LoadingQuery, copyJobHandle);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    [BurstCompile]
    struct CopyGettingOnIDs : IJobForEachWithEntity<TrainId>
    {
        [WriteOnly]
        public NativeArray<int> outputBuffer;

        public void Execute(Entity entity, int jobIndex, ref TrainId trainId)
        {
            outputBuffer[jobIndex] = trainId.value;
        }
    }

    struct ApplyLoadingTrainTransition : IJobForEachWithEntity<TrainId>
    {
        [DeallocateOnJobCompletion]
        public NativeArray<int> inputBuffer;
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex, [ReadOnly] ref TrainId trainId)
        {
            for (int i = 0; i < inputBuffer.Length; ++i) {
                if (inputBuffer[i] == trainId.value) { return; }
            }

            commandBuffer.RemoveComponent<LOADING>(jobIndex, entity);
            commandBuffer.AddComponent<DOORS_CLOSE>(jobIndex, entity);
        }
    }
}
