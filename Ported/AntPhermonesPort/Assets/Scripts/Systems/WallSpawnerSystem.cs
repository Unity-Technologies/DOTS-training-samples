using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class WallSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var cellMap = EntityManager.GetBuffer<CellMap>(GetSingletonEntity<CellMap>());
        var pheromoneMap = EntityManager.GetBuffer<PheromoneMap>(GetSingletonEntity<PheromoneMap>());

        var config = GetSingleton<Config>();
        uint seed = (uint)System.DateTime.Now.Ticks;
        var random = new Unity.Mathematics.Random(seed);
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in WallSpawner wallSpawner) =>
            {
                ecb.DestroyEntity(entity);

                var cellMapHelper = new CellMapHelper(cellMap, config.CellMapResolution, config.WorldSize);
                cellMapHelper.InitCellMap();
                cellMapHelper.InitBorders();
                var circlePattern = cellMapHelper.CreateCirclePattern(2);

                var pheromoneMapHelper = new PheromoneMapHelper(pheromoneMap, config.CellMapResolution, config.WorldSize);
                pheromoneMapHelper.InitPheromoneMap();

                for (int i = 0; i < config.RingCount; ++i)
                {
                    // choose if 2 openings
                    int segmentCount = random.NextInt(1, config.MaxEntriesPerRing);
                    float startAngle = random.NextFloat(0f, 360f);

                    for (int s = 0; s < segmentCount; ++s)
                    {
                        SpawnWallSegment(ecb, wallSpawner, cellMapHelper, circlePattern, config, i, startAngle, segmentCount);
                    }
                }
            }).Run();

        Entities
            .ForEach((Entity entity, in FoodSpawner foodSpawner) =>
            {
                ecb.DestroyEntity(entity);

                var instance = ecb.Instantiate(foodSpawner.FoodComponent);

                float angle = random.NextFloat(0f, 360f) *  Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * (config.RingCount + 1) * config.RingDistance;
                float y = Mathf.Sin(angle) * (config.RingCount + 1) * config.RingDistance;

                ecb.SetComponent(instance, new Translation { Value = new float3(x, 0, y) });
               
                var cellMapHelper = new CellMapHelper(cellMap, config.CellMapResolution, config.WorldSize);

                float2 xy = new float2(x, y);
                int cellIndex = cellMapHelper.grid.GetNearestIndex(xy);

                ecb.AddComponent(instance, new Food { Position = new float2(x, y), CellMapIndex = cellIndex });

                var circlePattern = cellMapHelper.CreateCirclePattern(2);

                cellMapHelper.grid.WorldToCellSpace(ref x, ref y);

                cellMapHelper.StampPattern((int)(x - (float)circlePattern.Length / 2), (int)(y - (float)circlePattern.Length / 2), circlePattern, CellState.IsFood);

                x = config.CellMapResolution / 2f;
                y = config.CellMapResolution / 2f;

                cellMapHelper.StampPattern((int)(x - (float)circlePattern.Length / 2), (int)(y - (float)circlePattern.Length / 2), circlePattern, CellState.IsNest);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static void SpawnWallSegment(EntityCommandBuffer ecb, WallSpawner wallSpawner, CellMapHelper cellMapHelper, NativeArray<int2> circlePattern, Config config, int ringIndex, float startAngle, int segmentCount)
    {
        float distance = (ringIndex + 1) * config.RingDistance;
        float angleSize = config.RingAngleSize / (float)segmentCount;
        float endAngle = startAngle + angleSize;

        for (float angle = startAngle; angle <= endAngle; angle += 10f / distance)
        {
            // get world space x, y
            float tmpAngle = angle;
            if (tmpAngle >= 360f)
                tmpAngle -= 360f;
            tmpAngle *= Mathf.Deg2Rad;
            float x = Mathf.Cos(tmpAngle) * distance;
            float y = Mathf.Sin(tmpAngle) * distance;

            var instance = ecb.Instantiate(wallSpawner.WallComponent);
            ecb.SetComponent(instance, new Translation { Value = new float3(x-0.2f, 0, y-0.6f) });

            // convert back. I know this is dumb, but I'm doing this quick
            cellMapHelper.grid.WorldToCellSpace(ref x, ref y);

            cellMapHelper.StampPattern((int)(x - (float)circlePattern.Length / 2),(int)( y - (float)circlePattern.Length / 2), circlePattern, CellState.IsObstacle);
        }
    }
}