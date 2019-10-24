using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class UnloadingTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    EntityQuery m_UnloadingQuery;
    EntityQuery m_GettingOffQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_UnloadingQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<UNLOADING>(),
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
        var gettingOffEntitiesCount = m_GettingOffQuery.CalculateEntityCount();
        var gettingOffIDs = new NativeArray<int>(gettingOffEntitiesCount, Allocator.TempJob);

        var copyJob = new CopyGettingOffIDs
        {
            outputBuffer = gettingOffIDs
        };

        var copyJobHandle = copyJob.Schedule(m_GettingOffQuery, inputDeps);

        var job = new ApplyUnloadingTrainTransition
        {
            inputBuffer = gettingOffIDs,
            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var handle = job.Schedule(m_UnloadingQuery, copyJobHandle);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    [BurstCompile]
    struct CopyGettingOffIDs : IJobForEachWithEntity<TrainId>
    {
        [WriteOnly]
        public NativeArray<int> outputBuffer;

        public void Execute(Entity entity, int jobIndex, ref TrainId trainId)
        {
            outputBuffer[jobIndex] = trainId.value;
        }
    }

    struct ApplyUnloadingTrainTransition : IJobForEachWithEntity<TrainId>
    {
        [DeallocateOnJobCompletion]
        public NativeArray<int> inputBuffer;
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex, [ReadOnly] ref TrainId trainId)
        {
            for (int i = 0; i < inputBuffer.Length; ++i)
            {
                if (inputBuffer[i] == trainId.value) { return; }
            }

            commandBuffer.RemoveComponent<UNLOADING>(jobIndex, entity);
            commandBuffer.AddComponent<LOADING>(jobIndex, entity);
        }
    }
}
