using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;


public class SpawnTestSystem : ComponentSystem
{
    private EntityQuery m_Query;
    private EntityQuery m_SpawnerQuery;
    private Random RandomGenerator;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(LbBoard));
        m_SpawnerQuery = GetEntityQuery(typeof(LbSpawnerTest));

        RandomGenerator = new Random();
        RandomGenerator.InitState(2);
    }

    protected override void OnUpdate()
    {
        var board = m_Query.GetSingleton<LbBoard>();

        var manager = World.Active.EntityManager;

        Entities.ForEach((ref LbSpawnerTest spawner) => 
        {
            for (int z=0; z<board.SizeY; ++z)
            {
                for (int x = 0; x<board.SizeX; ++x)
                {
                    DoSpawn(manager, spawner.Prefab, x, z);
                }
            }
        });

        manager.DestroyEntity(m_SpawnerQuery);
    }

    private void DoSpawn(EntityManager manager, Entity prefab, int x, int z)
    {
        var entity = manager.Instantiate(prefab);
        manager.SetComponentData(entity, new Translation() { Value = new float3(x, 1.0f, z) });
        manager.AddComponent<LbRat>(entity);
        manager.AddComponent<LbMovementTarget>(entity);
        manager.AddComponentData(entity, new LbMovementSpeed() { Value = 4.0f });
        manager.AddComponentData(entity, new LbDistanceToTarget() { Value = 1.0f });
        manager.AddComponentData(entity, new LbRotationSpeed() { Value = 1.0f });
        manager.AddComponentData(entity, new LbDirection() { Value = (byte)RandomGenerator.NextInt(4) });
    }
}
