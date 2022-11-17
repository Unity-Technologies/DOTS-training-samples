using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(MazeGeneratorSystem))]
[UpdateAfter(typeof(PlayerSpawnSystem))]
[UpdateAfter(typeof(PillSpawnSystem))]
[BurstCompile]
public partial struct ZombieSpawner : ISystem
{
    private bool _zombiesSpawned;
    private Random _random;
    private EntityQuery _pillQuery;

    private bool _initialized;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
        _pillQuery = state.GetEntityQuery(ComponentType.ReadOnly<Pill>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        
        if (!_initialized)
        {
            _initialized = true;
            _random = new Random(gameConfig.seed);
        }

        if (!_zombiesSpawned)
        {
            _zombiesSpawned = true;
            
            var player = SystemAPI.GetSingletonEntity<Player>();

            var pillArray = _pillQuery.ToEntityArray(Allocator.Temp);
            
            for (int i = 0; i < gameConfig.num_zombies; i++)
            {
                if (gameConfig.zombiePrefab == Entity.Null) 
                    break;
            
                var border = _random.NextInt(0, 5);
                var randomPos = _random.NextInt(0, gameConfig.mazeSize);
                
                var randomBorderPosition = GetBorderPosition(border, randomPos, gameConfig.mazeSize);

                var pos = MazeUtils.GridPositionToWorld(randomBorderPosition.x, randomBorderPosition.y);
                
                var zombie = state.EntityManager.Instantiate(gameConfig.zombiePrefab);
                state.EntityManager.SetComponentData(zombie, new LocalToWorldTransform
                {
                    Value = UniformScaleTransform.FromPosition(pos)
                });

                // path
                state.EntityManager.AddBuffer<Trajectory>(zombie);
                
                // Select target
                state.EntityManager.AddComponent<HunterTarget>(zombie);
                
                int capsuleIndex = (i % (gameConfig.numPills + 1)) - 1;
                if (capsuleIndex == -1) // Hunt player
                {
                    state.EntityManager.AddComponent<PlayerHunter>(zombie);
                    state.EntityManager.SetComponentData(zombie, new PlayerHunter()
                    {
                        player = player
                    });
                    state.EntityManager.SetComponentData(zombie, new HunterTarget()
                    {
                        position = SystemAPI.GetComponent<LocalToWorldTransform>(player).Value.Position
                    });
                }
                else // Hunt pill
                {
                    int pillIndex = _random.NextInt(0, pillArray.Length);
                    
                    state.EntityManager.AddComponent<PillHunter>(zombie);
                    state.EntityManager.SetComponentData(zombie, new HunterTarget()
                    {
                        position = SystemAPI.GetComponent<LocalToWorldTransform>(pillArray[pillIndex]).Value.Position
                    });
                }
                
                state.EntityManager.AddComponent<NeedUpdateTrajectory>(zombie);
                state.EntityManager.AddComponent<PositionInPath>(zombie);
                state.EntityManager.AddComponent<Speed>(zombie);
                state.EntityManager.AddComponent<NeedUpdateTarget>(zombie);
                state.EntityManager.SetComponentEnabled<NeedUpdateTarget>(zombie, false);
                state.EntityManager.SetComponentData(zombie, new Speed()
                {
                    speed = 2
                });
            }

            pillArray.Dispose();
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
