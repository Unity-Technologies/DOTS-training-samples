using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public partial class WallSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Unity.Mathematics.Random(1234);
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .ForEach((Entity entity, in WallSpawner wallSpawner) =>
            {
                ecb.DestroyEntity(entity);
                for (int i = 0; i < Config.RingCount; ++i)
                {
                    // choose if 2 openings
                    SpawnWallSegment(ecb, wallSpawner, 10 + (i * 10f), 0f, 270f);
                }

                
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static void SpawnWallSegment(EntityCommandBuffer ecb, WallSpawner wallSpawner, float distance, float startAngle, float endAngle, float stepAngle = 1.0f)
    {
        stepAngle *= Mathf.Deg2Rad;

        startAngle *= Mathf.Deg2Rad;
        endAngle *= Mathf.Deg2Rad;

        for (float angle = startAngle; angle <= endAngle; angle += stepAngle)
        {
            float x = Mathf.Cos(angle) * distance;
            float y = Mathf.Sin(angle) * distance;

            var instance = ecb.Instantiate(wallSpawner.WallComponent);
            ecb.SetComponent(instance, new Translation
            {
                Value = new float3(x, 0, y)
            });
        }
    }
}