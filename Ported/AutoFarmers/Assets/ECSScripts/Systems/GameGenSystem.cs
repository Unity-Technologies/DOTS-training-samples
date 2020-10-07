using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class GameGenSystem : SystemBase
{
    Random Random;
    private EntityCommandBufferSystem ECBSystem;

    protected override void OnCreate()
    {
        Random = new Random(42);
        ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameSpawn>();
    }

    protected override void OnUpdate()
    {
        var gameStateEntity = GetSingletonEntity<GameState>();
        var gameState = GetSingleton<GameState>();
        // Remove the game spawn component so that this system doesn't run again next frame
        EntityManager.RemoveComponent<GameSpawn>(gameStateEntity);

        // Init random plains and water tiles
        for (int y = 0; y < gameState.GridSize.y; y++)
        {
            for (int x = 0; x < gameState.GridSize.x; x++)
            {
                // Calculate tile position
                const float TILE_SIZE = 1f;
                var position = new Position { Value = new float2(x, y) * TILE_SIZE };
                // Randomly decide if this is a water tile or a plains tile
                bool isWater = (Random.NextFloat() < gameState.WaterProbability);
                // Instantiate the tile
                Entity tileEntity;
                {

                    Entity tilePrefab;
                    tilePrefab = isWater ? gameState.WaterPrefab : gameState.PlainsPrefab;
                    tileEntity = EntityManager.Instantiate(tilePrefab);
                    EntityManager.SetComponentData(tileEntity, new Translation { Value = new float3(x, 0f, y) * TILE_SIZE });
                    EntityManager.AddComponentData(tileEntity, position);
                    if (isWater)
                    {
                        EntityManager.AddComponent<Water>(tileEntity);
                    }
                    else
                    {
                        EntityManager.AddComponent<Plains>(tileEntity);
                        EntityManager.AddComponent<Tilled>(tileEntity); //NOTE(atheisen): farmers should add this, here to spawn crops while testing
                        EntityManager.AddComponent<MaterialOverride>(tileEntity);
                    }
                }
                // Randomly decide if this tile has a depot on it
                if (!isWater && Random.NextFloat() < gameState.DepotProbability)
                {
                    // Instantiate the depot
                    var depotEntity = EntityManager.Instantiate(gameState.DepotPrefab);
                    EntityManager.AddComponentData(depotEntity, position);
                    // Add the depot tag on the tile
                    EntityManager.AddComponent<Depot>(tileEntity);
                }
            }
        }

        // Build a list of water tile positions
        var waterTilePositions = new NativeList<float2>(Allocator.TempJob);
        Entities
            .WithName("build_water_list")
            .ForEach((in Water water, in Position position) =>
        {
            waterTilePositions.Add(position.Value);
        }).Run();

        // Compute the fertility of all the plain tiles in parallel
        Entities
            .WithName("calculate_fertility")
            .WithReadOnly(waterTilePositions)
            .WithDisposeOnCompletion(waterTilePositions)
            .ForEach((int entityInQueryIndex, ref Plains plains, ref MaterialOverride materialOverride, ref Tilled tilled, in Position position) =>
        {
            // Calculate the distance from the nearest water tile
            float minDistSq = float.MaxValue;
            for (int i = 0; i < waterTilePositions.Length; i++)
            {
                minDistSq = math.min(minDistSq, math.distancesq(position.Value, waterTilePositions[i]));
            }
            float minDist = math.sqrt(minDistSq);
            // Calculate the fertility based on this distance
            const float MAX_FERTILE_DISTANCE = 4f;
            float fertilityCoeff = math.max(0f, (MAX_FERTILE_DISTANCE - minDist) / MAX_FERTILE_DISTANCE); // In the range [0, 1]
            // Assign the color
            materialOverride.Value = math.lerp(new float4(1, 1, 1, 1), new float4(0.3f, 1, 0.3f, 1), fertilityCoeff);
            // Assign the fertility
            const int MAX_FERTILITY = 10;
            int fertility = (int)math.ceil(fertilityCoeff * MAX_FERTILITY);
            plains.Fertility = fertility;
            tilled.FertilityLeft = fertility; //NOTE(atheisen): farmers should add this, here to spawn crops while testing
        }).ScheduleParallel();

        // If very fertile and not on a depot, spawn forests
        var ecb = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithName("spawn_forests")
            .WithNone<Depot>()
            .ForEach((Entity entity, int entityInQueryIndex, in Plains plains, in Position position) =>
        {
            // Generate parallelizable uniform pseudo-randomness
            float random;
            {
                float2 p = position.Value;
                float3 p3 = math.frac(p.xyx * 0.1031f);
                p3 += math.dot(p3, p3.yzx + 33.33f);
                random = math.frac((p3.x + p3.y) * p3.z);
            }
            // Spawn forests if the fertility is greater than a threshold, and a certain probability condition is met
            const int FERTILITY_THRESHOLD = 3;
            if (plains.Fertility > FERTILITY_THRESHOLD && random < gameState.ForestProbability)
            {
                // Create the display entity that holds the forest mesh
                Entity displayEntity = ecb.Instantiate(entityInQueryIndex, gameState.ForestPrefab);
                ecb.AddComponent<Position>(entityInQueryIndex, displayEntity);
                ecb.SetComponent(entityInQueryIndex, displayEntity, position);
                // Add the display entity and the forest entity to the tile
                ecb.AddComponent(entityInQueryIndex, entity, new ForestDisplay { Value = displayEntity });
                ecb.AddComponent(entityInQueryIndex, entity, new Forest { Health = gameState.InitalForestHealth });
            }

        }).ScheduleParallel();

        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
