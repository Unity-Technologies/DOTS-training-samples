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

                UnityEngine.Time.timeScale = config.Speed;

                var cellMapHelper = new CellMapHelper(cellMap, config.CellMapResolution, config.WorldSize);
                cellMapHelper.InitCellMap();
                cellMapHelper.InitBorders();
                var wallPattern = cellMapHelper.CreateCirclePattern(2);

                var pheromoneMapHelper = new PheromoneMapHelper(pheromoneMap, config.CellMapResolution, config.WorldSize);
                pheromoneMapHelper.InitPheromoneMap();

                for (int i = 0; i < config.RingCount; ++i)
                {
                    // choose if 2 openings
                    int segmentCount = random.NextInt(1, config.MaxEntriesPerRing);
                    float startAngle = random.NextFloat(0f, 360f);

                    for (int s = 0; s < segmentCount; ++s)
                    {
                        SpawnWallSegment(ecb, wallSpawner, cellMapHelper, wallPattern, config, i, startAngle, segmentCount);
                    }
                }
            }).Run();

        Entities
            .ForEach((Entity entity, in FoodSpawner foodSpawner) =>
            {
                ecb.DestroyEntity(entity);

                var instance = ecb.Instantiate(foodSpawner.FoodComponent);

                float angle = random.NextFloat(0f, 360f) *  Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * 12;
                float y = Mathf.Sin(angle) * 12;

                ecb.SetComponent(instance, new Translation { Value = new float3(x, 0, y) });
               
                var cellMapHelper = new CellMapHelper(cellMap, config.CellMapResolution, config.WorldSize);

                float2 xy = new float2(x, y);
                int cellIndex = cellMapHelper.GetNearestIndex(xy);

                cellMapHelper.Set(cellIndex, CellState.IsFood);

                ecb.AddComponent(instance, new Food { Position = new float2(x, y), CellMapIndex = cellIndex });

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static void SpawnWallSegment(EntityCommandBuffer ecb, WallSpawner wallSpawner, CellMapHelper cellMapHelper, NativeArray<int2> wallPattern, Config config, int ringIndex, float startAngle, int segmentCount)
    {
        float distance = (ringIndex + 1) * config.RingDistance;
        float angleSize = config.RingAngleSize / (float)segmentCount;
        float endAngle = startAngle + angleSize;

        for (float angle = startAngle; angle <= endAngle; angle += 10f / distance)
        {
            float tmpAngle = angle;
            if (tmpAngle >= 360f)
                tmpAngle -= 360f;
            tmpAngle *= Mathf.Deg2Rad;
            float x = Mathf.Cos(tmpAngle) * distance;
            float y = Mathf.Sin(tmpAngle) * distance;

            var instance = ecb.Instantiate(wallSpawner.WallComponent);
            ecb.SetComponent(instance, new Translation { Value = new float3(x, 0, y) });

            cellMapHelper.WorldToCellSpace(ref x, ref y);

            cellMapHelper.StampPattern((int)(x - (float)wallPattern.Length / 2),(int)( y - (float)wallPattern.Length / 2), wallPattern);
        }
    }
}