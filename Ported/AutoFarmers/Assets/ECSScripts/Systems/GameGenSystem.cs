﻿using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class GameGenSystem : SystemBase
{
    Random random;
    protected override void OnCreate()
    {
        random = new Random(42);
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
                const float TILE_SIZE = 1f;
                Entity tilePrefab;
                bool isWater = (random.NextFloat() < gameState.WaterProbability);
                tilePrefab = isWater ? gameState.WaterPrefab : gameState.PlainsPrefab;

                var newTile = EntityManager.Instantiate(tilePrefab);
                EntityManager.SetComponentData(newTile, new Translation { Value = new float3(x, 0f, y) * TILE_SIZE });
                EntityManager.AddComponentData(newTile, new Position { Value = new float2(x, y) * TILE_SIZE });

                if (isWater)
                {
                    EntityManager.AddComponent<Water>(newTile);
                }
                else
                {
                    EntityManager.AddComponent<Plains>(newTile);
                    EntityManager.AddComponent<MaterialOverride>(newTile);
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
            .ForEach((ref Plains plains, ref MaterialOverride materialOverride, in Position position) =>
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
            plains.Fertility = (int)math.ceil(fertilityCoeff * MAX_FERTILITY);

        }).ScheduleParallel();
    }
}
