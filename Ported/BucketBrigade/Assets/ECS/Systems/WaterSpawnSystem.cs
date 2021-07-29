using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class WaterSpawnSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<GameConfigComponent>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp); 
        
        var gameConfig = GetSingleton<GameConfigComponent>();
        var waterPrefab = gameConfig.WaterPrefab;
        var waterPresenceProbability = gameConfig.WaterPresenceProbability;
        var waterOffset = gameConfig.WaterOffsetFromSimulation;
        var waterOffsetVariation = gameConfig.WaterOffsetVariation;
        var gridSize = gameConfig.SimulationSize;

        var rand = new Random(gameConfig.RandomSeed);

        var lowPlacementValue = -1 - waterOffset;
        var highPlacementValue = gridSize + waterOffset;
        
        for (var i = 0; i < gridSize; ++i)
        {
            // Bottom line
            if (rand.NextFloat() < waterPresenceProbability)
            {
                var waterEntity = ecb.Instantiate(waterPrefab);
                var offsetVariation = rand.NextFloat(-1.0f, 1.0f) * waterOffsetVariation;
                ecb.SetComponent(waterEntity, new Translation(){Value = new float3(lowPlacementValue + offsetVariation, 0, i)});
            }
            
            // Top line
            if (rand.NextFloat() < waterPresenceProbability)
            {
                var waterEntity = ecb.Instantiate(waterPrefab);
                var offsetVariation = rand.NextFloat(-1.0f, 1.0f) * waterOffsetVariation;
                ecb.SetComponent(waterEntity, new Translation(){Value = new float3(highPlacementValue + offsetVariation, 0, i)});
            }
            
            // Left column
            if (rand.NextFloat() < waterPresenceProbability)
            {
                var waterEntity = ecb.Instantiate(waterPrefab);
                var offsetVariation = rand.NextFloat(-1.0f, 1.0f) * waterOffsetVariation;
                ecb.SetComponent(waterEntity, new Translation(){Value = new float3(i, 0, lowPlacementValue + offsetVariation)});
            }
            
            // Right column
            if (rand.NextFloat() < waterPresenceProbability)
            {
                var waterEntity = ecb.Instantiate(waterPrefab);
                var offsetVariation = rand.NextFloat(-1.0f, 1.0f) * waterOffsetVariation;
                ecb.SetComponent(waterEntity, new Translation(){Value = new float3(i, 0, highPlacementValue + offsetVariation)});
            }
        }
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        // Run just once, unless reset elsewhere.
        Enabled = false;
    }
}