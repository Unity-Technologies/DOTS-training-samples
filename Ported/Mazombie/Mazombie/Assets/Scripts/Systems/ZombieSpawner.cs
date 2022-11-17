using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(MazeGeneratorSystem))]
[UpdateAfter(typeof(PlayerSpawnSystem))]
[BurstCompile]
public partial struct ZombieSpawner : ISystem
{
    private bool _zombiesSpawned;
    
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
        if (!_zombiesSpawned)
        {
            var gameConfig = SystemAPI.GetSingleton<GameConfig>();
            var random = new Random(gameConfig.seed);
            
            _zombiesSpawned = true;
            for (int i = 0; i < gameConfig.num_zombies; i++)
            {
                if (gameConfig.zombiePrefab == Entity.Null) 
                    break;
            
                var border = random.NextInt(0, 5);
                var randomPos = random.NextInt(0, gameConfig.mazeSize);
                
                var randomBorderPosition = GetBorderPosition(border, randomPos, gameConfig.mazeSize);

                var pos = MazeUtils.GridPositionToWorld(randomBorderPosition.x, randomBorderPosition.y);
                
                var zombie = state.EntityManager.Instantiate(gameConfig.zombiePrefab);
                state.EntityManager.SetComponentData(zombie, new LocalToWorldTransform
                {
                    Value = UniformScaleTransform.FromPosition(pos)
                });

                var player = SystemAPI.GetSingletonEntity<Player>();

                // path
                state.EntityManager.AddBuffer<Trajectory>(zombie);
                state.EntityManager.AddComponent<HunterTarget>(zombie);
                state.EntityManager.SetComponentData(zombie, new HunterTarget()
                {
                    target = player
                });
                state.EntityManager.AddComponent<NeedUpdateTrajectory>(zombie);
                state.EntityManager.AddComponent<PositionInPath>(zombie);
                state.EntityManager.AddComponent<Speed>(zombie);
                state.EntityManager.SetComponentData(zombie, new Speed()
                {
                    speed = 1
                });
            }
        }
        else
        {
            
            
        }
    }

    [BurstCompile]
    private int2 GetBorderPosition(int border, int random, int gridSize)
    {
        switch (border)
        {
            case 0:
                return new int2(0, random);
            case 1:
                return new int2(random, 0);
            case 2:
                return new int2(gridSize-1, random);
            case 3:
                return new int2(random, gridSize-1);
        }

        return new int2();
    }
}
