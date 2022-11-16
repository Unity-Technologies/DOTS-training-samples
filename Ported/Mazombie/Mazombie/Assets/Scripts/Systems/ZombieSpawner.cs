using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(MazeGeneratorSystem))]
[BurstCompile]
public partial struct ZombieSpawner : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();

        for (int i = 0; i < gameConfig.num_zombies; i++)
        {
            if (gameConfig.zombiePrefab != Entity.Null)
            {
                var zombie = state.EntityManager.Instantiate(gameConfig.zombiePrefab);
                state.EntityManager.SetComponentData(zombie, new LocalToWorldTransform
                {
                    Value = UniformScaleTransform.FromPosition(5,0,5)
                });
            
                // path
                state.EntityManager.AddBuffer<Trajectory>(zombie);
                state.EntityManager.AddComponent<NeedUpdateTrajectory>(zombie);   
            }
        }
        
        state.Enabled = false;
    }
}
