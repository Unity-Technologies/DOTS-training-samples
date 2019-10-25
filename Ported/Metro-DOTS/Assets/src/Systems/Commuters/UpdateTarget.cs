using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

class UpdateTarget : JobComponentSystem
{
    EntityCommandBufferSystem m_ECBSystem;
    EntityQuery m_Query;
    EntityQuery m_PlatformPosQuery;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_PlatformPosQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new [] { ComponentType.ReadOnly<PlatformTransforms>() },
        });
        m_Query = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<CurrentPathIndex>(),
                    ComponentType.ReadOnly<PathLookup>()
                },
                None = new []
                {
                    ComponentType.ReadOnly<TargetPosition>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        PlatformTransforms tr;
        using (var trs = m_PlatformPosQuery.ToEntityArray(Allocator.TempJob))
            tr = EntityManager.GetComponentData<PlatformTransforms>(trs[0]);

        var handle = new UpdateTargetJob
        {
            commandBuffer = m_ECBSystem.CreateCommandBuffer().ToConcurrent(),
            platformTransforms = tr
        }.Schedule(m_Query, inputDeps);

        m_ECBSystem.AddJobHandleForProducer(handle);

        return handle;
    }

    struct UpdateTargetJob : IJobForEachWithEntity<PathLookup, CurrentPathIndex>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        [ReadOnly]
        public PlatformTransforms platformTransforms;

        public void Execute(Entity entity, int jobIndex, [ReadOnly] ref PathLookup pathLookup, ref CurrentPathIndex pathIndex)
        {
            var path = pathLookup.value.Value.GetPath(pathIndex.pathLookupIdx);
            pathIndex.connectionIdx++;

            if (pathIndex.connectionIdx >= path.connections.Length)
            {
                // That was the last step
                commandBuffer.DestroyEntity(jobIndex, entity);
                return;
            }

            var connection = path.connections[pathIndex.connectionIdx];
            var dest = platformTransforms.value.Value.translations[connection.destinationPlatformId];
            commandBuffer.AddComponent(jobIndex, entity, new TargetPosition {value = dest});
        }
    }
}