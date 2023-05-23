using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct BeeSpawnerSystem : ISystem
{
    private uint _updateCounter;

    private bool haveBeesSpawned;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (haveBeesSpawned)
        {
            return;
        }

        var config = SystemAPI.GetSingleton<Config>();
        var random = Random.CreateFromIndex(_updateCounter++);

        foreach (var (spawner, spawnerTransform) in SystemAPI.Query<BeeSpawnerComponent, LocalTransform>())
        {
            for (int i = 0; i < config.beeCount; i++)
            {
                Entity newBee = state.EntityManager.Instantiate(spawner.beePrefab);

                state.EntityManager.SetComponentData(newBee, new LocalTransform
                {
                    Position = spawnerTransform.Position,
                    Rotation = spawnerTransform.Rotation,
                    Scale = 1
                });

                state.EntityManager.SetComponentData(newBee, new VelocityComponent
                {
                    Velocity = random.NextFloat3Direction() * config.maxSpawnSpeed
                });

                state.EntityManager.SetComponentData(newBee, new ReturnHomeComponent
                {
                    HomeMinBounds = spawner.minBounds,
                    HomeMaxBounds = spawner.maxBounds
                });

                state.EntityManager.SetComponentData(newBee, new BeeState
                {
                    state = BeeState.State.IDLE,
                    hiveId = spawner.hiveId
                });

            }
        }

        haveBeesSpawned = true;

        // spawn new bees when food is placed in hive
        // remove food that has been placed
    }
}
