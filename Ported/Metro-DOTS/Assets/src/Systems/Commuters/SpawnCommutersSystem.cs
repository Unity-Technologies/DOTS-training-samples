using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct SpawnCommuters : IComponentData
{
    public int numberOfCommuters;
    public Entity prefab;
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class SpawnCommuterSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;
    EntityQuery m_PlatformPosQuery;
    EntityQuery m_PathLookupQuery;
    protected override void OnCreate()
    {
        m_PlatformPosQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new [] { ComponentType.ReadOnly<PlatformTransforms>() },
        });

        m_PathLookupQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new [] { ComponentType.ReadOnly<PathLookup>() },
        });
        m_Barrier = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        PlatformTransforms tr;
        using (var trs = m_PlatformPosQuery.ToEntityArray(Allocator.TempJob))
            tr = EntityManager.GetComponentData<PlatformTransforms>(trs.First());

        PathLookup pathLookup;
        using (var pathLookupEntities = m_PathLookupQuery.ToEntityArray(Allocator.TempJob))
            pathLookup = EntityManager.GetComponentData<PathLookup>(pathLookupEntities.First());

        var job = new SpawnCommutersJob { cmdBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent(), trs = tr, lookup = pathLookup}.Schedule(this, inputDeps);
        m_Barrier.AddJobHandleForProducer(job);
        return job;
    }

    struct SpawnCommutersJob : IJobForEachWithEntity<SpawnCommuters>
    {
        [ReadOnly] public PlatformTransforms trs;
        [WriteOnly] public EntityCommandBuffer.Concurrent cmdBuffer;
        [ReadOnly] public PathLookup lookup;

        public void Execute(Entity entity, int index, [ReadOnly] ref SpawnCommuters c0)
        {
            var random = new Random(123);
            var length = lookup.value.Value.paths.Length;
            for (var i = 0; i < c0.numberOfCommuters; i++)
            {
                var commuter = cmdBuffer.Instantiate(index, c0.prefab);
                var randomLookup = random.NextInt(0, length-1);
                var path = lookup.value.Value.paths[randomLookup];
                var platformId = path.fromPlatformId;
                var platformPos = trs.value.Value.translations[platformId];

                var destID = path.toPlatformId;
                var destPos = trs.value.Value.translations[destID];

                cmdBuffer.SetComponent(index, commuter, new Translation { Value = platformPos});
                cmdBuffer.AddComponent(index, commuter, new PlatformId { value = platformId });
                cmdBuffer.AddComponent(index, commuter, new CurrentPathIndex{ pathLookupIdx = randomLookup, connectionIdx = 0 });
                cmdBuffer.AddComponent(index, commuter, lookup);
                cmdBuffer.AddComponent(index, commuter, new TargetPosition { value = destPos});
                cmdBuffer.AddComponent(index, commuter, new Speed { value = 6.66f});
            }
            cmdBuffer.RemoveComponent(index, entity, typeof(SpawnCommuters));
        }
    }
}

