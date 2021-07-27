using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class ObstacleSpawnerSystem : SystemBase
{
    const int mapSize = 128;

    protected override void OnUpdate()
    {
        using (var ecb = new EntityCommandBuffer(Allocator.Temp))
        {
            Random rand = new Random(1234);

            Entities
                .ForEach((Entity entity, in ObstacleSpawner spawner) =>
                {
                    // Add logic for creating the obstacle rings
                    ecb.DestroyEntity(entity);

                    for (int i = 0; i < spawner.obstacleRingCount; i++)
                    {
                        float ringRadius = (i / (spawner.obstacleRingCount + 1f)) * (mapSize * .5f);
                        float circumference = ringRadius * 2f * math.PI;
                        int maxCount = (int)math.ceil(circumference / (2f * spawner.ObstacleRadius) * 2f);
                        int offset = rand.NextInt(0, maxCount);
                        int holeCount = rand.NextInt(1, 3);
                        for (int j = 0; j < maxCount; j++)
                        {
                            float t = (float)j / maxCount;
                            if ((t * holeCount) % 1f < spawner.ObstaclesPerRing)
                            {
                                float angle = (j + offset) / (float)maxCount * (2f * math.PI);

                                var obstacleEntity = ecb.Instantiate(spawner.ObstaclePrefab);
                                ecb.SetComponent(obstacleEntity, new Translation
                                {
                                    Value = new float3(mapSize * .5f + math.cos(angle) * ringRadius, mapSize * .5f + math.sin(angle) * ringRadius, 0)
                                });
                                //ecb.AddComponent(obstacleEntity, new Scale
                                //{
                                //    Value = spawner.ObstacleRadius
                                //});
                            }
                        }
                    }

                }).Run();


            ecb.Playback(EntityManager);
        }
    }
}
