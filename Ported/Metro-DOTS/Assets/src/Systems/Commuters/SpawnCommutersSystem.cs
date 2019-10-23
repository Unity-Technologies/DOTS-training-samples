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

[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
class MetroCommuterReferenceDeclaration : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Metro metro) =>
        {
            DeclareReferencedPrefab(metro.prefab_commuter);
        });
    }
}

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
        m_Barrier = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var trs = m_PlatformPosQuery.ToEntityArray(Allocator.TempJob);
        var tr = EntityManager.GetComponentData<PlatformTransforms>(trs.First());

        var pathLookupEntities = m_PathLookupQuery.ToEntityArray(Allocator.TempJob);
        var pathLookup = EntityManager.GetComponentData<PathLookup>(pathLookupEntities.First());

        trs.Dispose();
        pathLookupEntities.Dispose();

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
                var platform = cmdBuffer.Instantiate(index, c0.prefab);
                var randomLookup = random.NextInt(0, length-1);
                var platformId = lookup.value.Value.paths[randomLookup].fromPlatformId;
                var platformPos = trs.value.Value.translations[platformId];
                cmdBuffer.SetComponent(index, platform, new Translation { Value = platformPos});
                cmdBuffer.AddComponent(index, platform, new PlatformId { value = (uint)platformId });
            }
            cmdBuffer.RemoveComponent(index, entity, typeof(SpawnCommuters));
        }
    }
}

