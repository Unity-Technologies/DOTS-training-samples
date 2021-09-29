using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public partial class WallSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var cellMap = EntityManager.GetBuffer<CellMap>(GetSingletonEntity<CellMap>());
        var pheromoneMap = EntityManager.GetBuffer<PheromoneMap>(GetSingletonEntity<PheromoneMap>());

        var config = GetSingleton<Config>();
        var random = new Unity.Mathematics.Random(config.RandomSeed);
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .ForEach((Entity entity, in WallSpawner wallSpawner) =>
            {
                ecb.DestroyEntity(entity);

                var cellMapHelper = new CellMapHelper(cellMap, config.CellMapResolution, config.WorldSize);
                cellMapHelper.InitCellMap();
                cellMapHelper.InitBorders();
                int2[] wallPattern = cellMapHelper.CreateCirclePattern(10);

                var pheromoneMapHelper = new PheromoneMapHelper(pheromoneMap, config.CellMapResolution, config.WorldSize);
                pheromoneMapHelper.InitPheromoneMap();

                for (int i = 0; i < config.RingCount; ++i)
                {
                    // choose if 2 openings
                    int segmentCount = random.NextInt(1, config.MaxEntriesPerRing);
                    float startAngle = random.NextFloat(0f, 360f);
                    float angleSize = config.RingAngleSize / (float)segmentCount;

                    for (int s = 0; s < segmentCount; ++s)
                    {
                        SpawnWallSegment(ecb, wallSpawner, cellMapHelper, wallPattern, (i + 1) * config.RingDistance, startAngle, startAngle+angleSize);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static void SpawnWallSegment(EntityCommandBuffer ecb, WallSpawner wallSpawner, CellMapHelper cellMapHelper, int2[] wallPattern, float distance, float startAngle, float endAngle, float stepAngle = 10f)
    {
        for (float angle = startAngle; angle <= endAngle; angle += stepAngle / distance)
        {
            float tmpAngle = angle;
            if (tmpAngle >= 360f)
                tmpAngle -= 360f;
            tmpAngle *= Mathf.Deg2Rad;
            float x = Mathf.Cos(tmpAngle) * distance;
            float y = Mathf.Sin(tmpAngle) * distance;

            var instance = ecb.Instantiate(wallSpawner.WallComponent);
            ecb.SetComponent(instance, new Translation { Value = new float3(x, 0, y) });
            cellMapHelper.StampPattern((int)x, (int)y, wallPattern);
        }
    }
}