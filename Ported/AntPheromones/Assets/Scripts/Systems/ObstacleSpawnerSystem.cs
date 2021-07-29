using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class ObstacleSpawnerSystem : SystemBase
{
    const int mapSize = 128;
    const int bucketResolution = 32;
    protected override void OnUpdate()
    {
        using (var ecb = new EntityCommandBuffer(Allocator.Temp))
        {
            Random rand = new Random(1234);
            
            int bucketResSq = bucketResolution * bucketResolution;

            var bucketIndicesEntity = ecb.CreateEntity();
            ecb.AddComponent<ObstacleBucketEntity>(bucketIndicesEntity);
            var bucketIndicesBuffer = ecb.AddBuffer<ObstacleBucketIndices>(bucketIndicesEntity);
            bucketIndicesBuffer.Length = bucketResSq;

            for (int i = 0; i < bucketResSq; ++i)
            {
                bucketIndicesBuffer[i] = ecb.CreateEntity();
                ecb.AddBuffer<ObstacleBucket>(bucketIndicesBuffer[i]);
            }
            
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

                                float x = mapSize * .5f + math.cos(angle) * ringRadius;
                                float y = mapSize * .5f + math.sin(angle) * ringRadius;

                                var obstacleEntity = ecb.Instantiate(spawner.ObstaclePrefab);

                                var translation = new Translation
                                {
                                    Value = new float3(x, y, 0)
                                };
                                ecb.SetComponent(obstacleEntity, translation);
                                ecb.AddComponent(obstacleEntity, new Scale
                                {
                                    Value = spawner.ObstacleRadius
                                });


                                var pos = new float2 {
                                    x = translation.Value.x,
                                    y = translation.Value.y
                                };
                                float radius = spawner.ObstacleRadius;

                                float posX = translation.Value.x;
                                float posY = translation.Value.y;
                                for (int xx = (int)((posX - radius) / mapSize * bucketResolution); xx <= (int)((posX + radius) / mapSize * bucketResolution); ++xx)
                                {
                                    if (x < 0 || x > bucketResolution)
                                    {
                                        continue;
                                    }

                                    for (int yy = (int)((posY - radius) / mapSize * bucketResolution); yy <= (int)((posY + radius) / mapSize * bucketResolution); ++yy)
                                    {
                                        if (y < 0 || y > bucketResolution)
                                        {
                                            continue;
                                        }
                                        int xxx = (int)(xx / mapSize * bucketResolution);
                                        int yyy = (int)(yy / mapSize * bucketResolution);
                                        int bucketEntityIndex = xxx * bucketResolution + yyy;
                                        var bucketEntity = bucketIndicesBuffer[bucketEntityIndex].Value;
                                        var bucket = GetBuffer<ObstacleBucket>(bucketEntity);
                                        bucket.Add(translation);
                                    }
                                }
                                
                            }
                        }
                    }

                }).Run();
            
            ecb.Playback(EntityManager);
        }


        }
}
