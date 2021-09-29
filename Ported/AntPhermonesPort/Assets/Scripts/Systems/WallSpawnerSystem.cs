using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public partial class WallSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var map = EntityManager.GetBuffer<CellMap>(GetSingletonEntity<CellMap>());

        var random = new Unity.Mathematics.Random(1234);
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var config = GetSingleton<Config>();
        Entities
            .ForEach((Entity entity, in WallSpawner wallSpawner) =>
            {
                ecb.DestroyEntity(entity);

                var cellMapHelper = new CellMapHelper(map, config.CellMapResolution, config.WorldSize);
                cellMapHelper.InitCellMap();
                cellMapHelper.InitBorders();

                for (int i = 0; i < config.RingCount; ++i)
                {
                    // choose if 2 openings
                    int segmentCount = random.NextInt(1, config.MaxEntriesPerRing);
                    float startAngle = random.NextFloat(0f, 360f);
                    float angleSize = config.RingAngleSize / (float)segmentCount;

                    for (int s = 0; s < segmentCount; ++s)
                    {
                        SpawnWallSegment(ecb, wallSpawner, (i + 1) * config.RingDistance, startAngle, startAngle+angleSize);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static void SpawnWallSegment(EntityCommandBuffer ecb, WallSpawner wallSpawner, float distance, float startAngle, float endAngle, float stepAngle = 1f)
    {
        for (float angle = startAngle; angle <= endAngle; angle += stepAngle)
        {
            float tmpAngle = angle;
            if (tmpAngle >= 360f)
                tmpAngle -= 360f;
            tmpAngle *= Mathf.Deg2Rad;
            float x = Mathf.Cos(tmpAngle) * distance;
            float y = Mathf.Sin(tmpAngle) * distance;

            var instance = ecb.Instantiate(wallSpawner.WallComponent);
            ecb.SetComponent(instance, new Translation
            {
                Value = new float3(x, 0, y)
            });
        }
    }
}