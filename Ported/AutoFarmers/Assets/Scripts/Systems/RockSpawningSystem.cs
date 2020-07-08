using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RockSpawningSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private Random m_Random;

    // Set on spawners to make them go to sleep after init
    public struct RocksAreInitalizedTag : IComponentData
    {
    };

    protected override void OnCreate()
    {
        m_Random = new Random(0x1234567);
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        var random = m_Random;

        Entities
        .WithNone<RocksAreInitalizedTag>()
        .ForEach((Entity rockSpawner, in RockSpawner spawner) =>
        {
            for (int n=0; n<spawner.NumRocks; n++)
            {
                int2 size = random.NextInt2(spawner.RandomSizeMin, spawner.RandomSizeMax);
                int2 position = random.NextInt2(new int2(0, 0), new int2(64, 64));
                var instance = ecb.Instantiate(spawner.RockPrefab);
                var translation = new float3(position.x, 0.0f, position.y);
                var scale = new float3(size.x, 1.0f, size.y);
                translation += scale * 0.5f;
                ecb.SetComponent(instance, new Translation { Value = translation });
                ecb.AddComponent(instance, new NonUniformScale { Value = scale });
                ecb.AddComponent(instance, new CellPosition { Value = position });
                ecb.AddComponent(instance, new CellSize { Value = size });
                ecb.AddComponent(instance, new Rock_Tag());

                float area = size.x * size.y;
                float healthPerArea = random.NextFloat(spawner.minHealthPerArea, spawner.maxHealthPerArea);
                float health = area * healthPerArea;

                ecb.AddComponent(instance, new Health { Value = health });
            }
            ecb.AddComponent(rockSpawner, new RocksAreInitalizedTag());
        }).Schedule();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}