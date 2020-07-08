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
            .WithAll<Height>()
            .ForEach((Entity entity) =>
            {
                ecb.DestroyEntity(entity);
            }).Schedule();

        // Clean up bombs
        Entities
            .WithAll<Position>()
            .WithNone<Height>()
            .WithNone<Rotation>()
            .WithNone<PlayerTag>()
            .ForEach((Entity entity) => 
            {
                ecb.DestroyEntity(entity);
            }).Schedule();

        // Clean up cannons
        Entities
            .WithAll<Position>()
            .WithAll<Rotation>()
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
                    var height = gameParams.TerrainMin + random.NextFloat() * (gameParams.TerrainMax - gameParams.TerrainMin);
                    GridHeight tileHeight;
                    tileHeight.Height = height;
                    tileHeightsBuffer[y * dimension.x + x] = tileHeight;
                    ecb.SetComponent(instance, new Position { Value = new float3(x, 0, y) });
                    ecb.SetComponent(instance, new Height { Value = height });

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
                    var instance = ecb.Instantiate(gameParams.CannonPrefab);
                    var pos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());
                    while (tilesOccupiedBuffer[pos.y * dimension.x + pos.x].Occupied)
                        pos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());

                    ecb.SetComponent(instance, new Position { Value = new float3(pos.x, tileHeightsBuffer[pos.y * dimension.x + pos.x].Height + k_CannongHeightOffset, pos.y) });
                    ecb.SetComponent(instance, new Rotation { Value = 2f * random.NextFloat() * math.PI});
                    ecb.SetComponent(instance, new Cooldown { Value = random.NextFloat() * gameParams.CannonCooldown });
                    tilesOccupiedBuffer[pos.y * dimension.x + pos.x] = new GridOccupied { Occupied = true };
                }
            }

            // Spawn player // NOT WORKING.
            {
                var instance = ecb.Instantiate(gameParams.PlayerPrefab);
                var pos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());
                while (tilesOccupiedBuffer[pos.y * dimension.x + pos.x].Occupied)
                    pos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());

                ecb.SetComponent(instance, new Position { Value = new float3(pos.x, tileHeightsBuffer[pos.y * dimension.x + pos.x].Height + 0.5f, pos.y) });
            }


            // Remove GameOverTag
            ecb.RemoveComponent<GameOverTag>(entity);
        }).Schedule();

        m_Random = random;

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}