using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class WaitForStopTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    EntityQuery m_UnloadingQuery;
    EntityQuery m_WaitingQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_UnloadingQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<UNLOADING>(),
                    ComponentType.ReadOnly<PlatformId>(),
                    ComponentType.ReadOnly<TrainId>()
                }
            });

        m_WaitingQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<WAIT_FOR_STOP>(),
                    ComponentType.ReadOnly<TrainId>(),
                    ComponentType.ReadOnly<CurrentPathIndex>(),
                    ComponentType.ReadOnly<PathLookup>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var unloadingIDsCount = m_UnloadingQuery.CalculateEntityCount();
        var unloadingTrainToStationIDs = new NativeArray<TrainToPlatform>(unloadingIDsCount, Allocator.TempJob);

        // Step 1. Get all the indices of entities that are tagged with LOADING
        var copyJob = new CopyUnloadingIDs
        {
            outputBuffer = unloadingTrainToStationIDs
        };

        var copyJobHandle = copyJob.Schedule(m_UnloadingQuery, inputDeps);

        // Step 2. Compare every entity tagged with QUEUE on their PlatformId with all the
        // stored entities that are LOADING.
        var job = new ApplyQueueTransition
        {
            inputBuffer = unloadingTrainToStationIDs,
            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var handle = job.Schedule(m_WaitingQuery, copyJobHandle);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    struct TrainToPlatform
    {
        public int trainID;
        public int platformID;

        public TrainToPlatform(int trainID, int platformID)
        {
            this.trainID = trainID;
            this.platformID = platformID;
        }
    }

    [BurstCompile]
    struct CopyUnloadingIDs : IJobForEachWithEntity<TrainId, PlatformId>
    {
        [WriteOnly]
        public NativeArray<TrainToPlatform> outputBuffer;

        public void Execute(Entity entity, int jobIndex, ref TrainId trainID, ref PlatformId platformID)
        {
            outputBuffer[jobIndex] = new TrainToPlatform(trainID.value, platformID.value);
        }
    }

    struct ApplyQueueTransition : IJobForEachWithEntity<TrainId, PathLookup, CurrentPathIndex>
    {
        [DeallocateOnJobCompletion]
        public NativeArray<TrainToPlatform> inputBuffer;
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex,
            ref TrainId trainID,
            ref PathLookup pathLookup,
            ref CurrentPathIndex pathIndex)
        {
            for (int i = 0; i < inputBuffer.Length; ++i)
            {
                if (inputBuffer[i].trainID == trainID.value)
                {
                    if (inputBuffer[i].platformID == pathLookup.value.Value.paths[pathIndex.connectionIdx].toPlatformId)
                    {
                        ++pathIndex.connectionIdx;
                        if (pathIndex.connectionIdx >= pathLookup.value.Value.paths.Length)
                        {
                            //TODO Depawn commuter
                        }
                        commandBuffer.RemoveComponent<WAIT_FOR_STOP>(jobIndex, entity);
                        commandBuffer.AddComponent<GET_OFF_TRAIN>(jobIndex, entity);
                    }
                }
            }
        }
    }
}
