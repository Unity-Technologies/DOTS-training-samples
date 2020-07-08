using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class GameInitSystem : SystemBase
{
    const float k_CannongHeightOffset = .3f;

    EntityCommandBufferSystem m_ECBSystem;
    EntityQuery m_Query;
    Random m_Random;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new ComponentType(typeof(GameOverTag)));
        RequireForUpdate(m_Query);

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

        // Setup the board
        Entities
            .ForEach((int entityInQueryIndex, Entity entity, in GameParams gameParams) =>
        {
            var dimension = gameParams.TerrainDimensions;
            var tileHeights = new NativeArray<float>(dimension.x * dimension.y, Allocator.Temp);

            // Spawn Tiles
            for (int y = 0; y < dimension.y; ++y)
            {
                for (int x = 0; x < dimension.x; ++x)
                {
                    var instance = ecb.Instantiate(gameParams.TilePrefab);
                    var height = gameParams.TerrainHeightRange.x + random.NextFloat() * (gameParams.TerrainHeightRange.y - gameParams.TerrainHeightRange.x);
                    tileHeights[y* dimension.x + x] = height;
                    ecb.SetComponent(instance, new Position { Value = new float3(x, 0, y) });
                    ecb.SetComponent(instance, new Height { Value = height });
                }
            }

            // Spawn Cannons
            var occupiedSlots = new NativeList<int2>(gameParams.CannonCount, Allocator.Temp);
            {
                for (int i = 0; i < gameParams.CannonCount; ++i)
                {
                    var instance = ecb.Instantiate(gameParams.CannonPrefab);
                    var pos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());
                    while (occupiedSlots.Contains(pos))
                        pos = (int2)(gameParams.TerrainDimensions * random.NextFloat2());

                    ecb.SetComponent(instance, new Position { Value = new float3(pos.x, tileHeights[pos.y * dimension.x + pos.x] + k_CannongHeightOffset, pos.y) });
                    ecb.SetComponent(instance, new Rotation { Value = 2f * random.NextFloat() * math.PI});
                    ecb.SetComponent(instance, new Cooldown { Value = random.NextFloat() * gameParams.CannonCooldown });
                    occupiedSlots.Add(pos);
                }
            }

            occupiedSlots.Dispose();
            tileHeights.Dispose();

            // Remove GameOverTag
            ecb.RemoveComponent<GameOverTag>(entity);
        }).Schedule();

        m_Random = random;

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}