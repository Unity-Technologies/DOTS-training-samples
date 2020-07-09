using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class GameInitSystem : SystemBase
{
    const float k_CannongHeightOffset = .3f;

    EntityCommandBufferSystem m_ECBSystem;
    EntityQuery m_Query;
    EntityQuery m_GridQuery;
    Random m_Random;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new ComponentType(typeof(GameOverTag)));
        RequireForUpdate(m_Query);

        m_GridQuery = GetEntityQuery(new ComponentType(typeof(GridTag)));

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        m_Random = new Random(0x1234567);
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer();
        var random = m_Random;

        // Clean up tiles
        Entities
            .WithAll<Position>()
            .WithAll<Color>()
            .ForEach((Entity entity) =>
            {
                ecb.DestroyEntity(entity);
            }).Schedule();

        // Clean up bombs
        Entities
            .WithAll<Position>()
            .WithNone<Color>()
            .WithNone<LookAtPlayerTag>()
            .WithNone<PlayerTag>()
            .ForEach((Entity entity) =>
            {
                ecb.DestroyEntity(entity);
            }).Schedule();

        // Clean up cannons
        Entities
            .WithAll<Position>()
            .WithAll<LookAtPlayerTag>()
            .ForEach((Entity entity) =>
            {
                ecb.DestroyEntity(entity);
            }).Schedule();

        // Clean up players
        Entities
            .WithAll<PlayerTag>()
            .ForEach((Entity entity) =>
            {
                ecb.DestroyEntity(entity);
            }).Schedule();


        // We destroy the grid as it will be recreated below.
        Entities.ForEach((Entity entity, in GridTag tag) =>
        {
            ecb.DestroyEntity(entity);
        }).Schedule();

        // Setup the board
        Entities
            .ForEach((int entityInQueryIndex, Entity entity, in GameParams gameParams) =>
        {
            var dimension = gameParams.TerrainDimensions;
            var gridEntity = ecb.CreateEntity();
            ecb.AddComponent<GridTag>(gridEntity);
            var tileHeightsBuffer = ecb.AddBuffer<GridHeight>(gridEntity);
            tileHeightsBuffer.EnsureCapacity(dimension.x * dimension.y);
            var tilesOccupiedBuffer = ecb.AddBuffer<GridOccupied>(gridEntity);
            tilesOccupiedBuffer.EnsureCapacity(dimension.x * dimension.y);

            // Clear the tiles. 
            for (int y = 0; y < dimension.y; ++y)
            {
                for (int x = 0; x < dimension.x; ++x)
                {
                    tileHeightsBuffer.Add(new GridHeight { Height = 0.0f });
                    tilesOccupiedBuffer.Add(new GridOccupied { Occupied = false });
                }
            }



            // Spawn Tiles
            for (int y = 0; y < dimension.y; ++y)
            {
                for (int x = 0; x < dimension.x; ++x)
                {
                    var instance = ecb.Instantiate(gameParams.TilePrefab);
                    //var height = math.lerp(gameParams.TerrainMin, gameParams.TerrainMax, (y * dimension.x + x)/(float)(dimension.x * dimension.y));
                    var height = gameParams.TerrainMin + random.NextFloat() * (gameParams.TerrainMax - gameParams.TerrainMin);
                    GridHeight tileHeight;
                    tileHeight.Height = height;
                    tileHeightsBuffer[GridFunctions.GetGridIndex(math.float2(x,y), gameParams.TerrainDimensions)] = tileHeight;
                    ecb.SetComponent(instance, new Position { Value = new float3(x, 0, y) });
                    ecb.SetComponent(instance, new Color { Value = math.float4(1.0f, 0.0f, 0.0f, 1.0f) });

                    float range = (gameParams.TerrainMax - gameParams.TerrainMin);
                    float value = (height - gameParams.TerrainMin) / range;
                    float4 color = math.lerp(gameParams.colorA, gameParams.colorB, value);

                    ecb.SetComponent(instance, new Color { Value = color });
                }
            }

            // Spawn Cannons
            {
                for (int i = 0; i < gameParams.CannonCount; ++i)
                {
                    var cannonBarrel = ecb.Instantiate(gameParams.CannonBarrel);
                    var tilePos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());
                    while (tilesOccupiedBuffer[tilePos.y * dimension.x + tilePos.x].Occupied)
                        tilePos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());

                    var position = new float3(tilePos.x, tileHeightsBuffer[GridFunctions.GetGridIndex(tilePos.xy, gameParams.TerrainDimensions)].Height + k_CannongHeightOffset, tilePos.y);

                    ecb.SetComponent(cannonBarrel, new Position { Value = position });
                    ecb.SetComponent(cannonBarrel, new Rotation { Value = -0.5f * math.PI });
                    ecb.SetComponent(cannonBarrel, new Cooldown { Value = random.NextFloat() * gameParams.CannonCooldown });

                    var cannonBase = ecb.Instantiate(gameParams.CannonPrefab);
                    ecb.AddComponent(cannonBase, new Position { Value = position });
                    ecb.AddComponent(cannonBase, new LookAtPlayerTag());

                    tilesOccupiedBuffer[GridFunctions.GetGridIndex(tilePos.xy, gameParams.TerrainDimensions)] = new GridOccupied { Occupied = true };
                }
            }

            // Spawn player 
            {
                var instance = ecb.Instantiate(gameParams.PlayerPrefab);
                var pos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());
                while (tilesOccupiedBuffer[GridFunctions.GetGridIndex(pos.xy, gameParams.TerrainDimensions)].Occupied)
                    pos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());

                float3 position = new float3(pos.x + 0.5f, tileHeightsBuffer[GridFunctions.GetGridIndex(pos.xy, gameParams.TerrainDimensions)].Height + PlayerNextMoveSystem.kYOffset, pos.y + 0.5f);
                MovementParabola movement = new MovementParabola();
                PlayerNextMoveSystem.InitPlayerPosition(ref movement, position, tileHeightsBuffer, gameParams);
                ecb.SetComponent(instance, new Position { Value = position });
                ecb.SetComponent(instance, movement);
                ecb.SetComponent(instance, new NormalisedMoveTime { Value = 0.0f });
            }
        }).Schedule();

        // Remove GameOverTag
        Entities
            .WithAll<GameOverTag>()
            .ForEach((Entity entity) =>
            {
                ecb.RemoveComponent<GameOverTag>(entity);
            }).Schedule();

        m_Random = random;

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}