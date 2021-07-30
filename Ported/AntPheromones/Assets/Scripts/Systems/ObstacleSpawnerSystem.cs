using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class ObstacleSpawnerSystem : SystemBase
{
    const int bucketResolution = 50;
    int skip = 10;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(ObstacleBucketEntity));
        RequireForUpdate(query);

        //Entity obstacleBucketEntity = GetSingletonEntity<ObstacleBucketEntity>();
        using (var ecb = new EntityCommandBuffer(Allocator.Temp))
        {
            var obstacleBucketEntity = ecb.CreateEntity();
            ecb.AddComponent<ObstacleBucketEntity>(obstacleBucketEntity);
            var bucketIndicesBuffer = ecb.AddBuffer<ObstacleBucketIndices>(obstacleBucketEntity);
            int bucketResolutionSq = bucketResolution * bucketResolution;
            bucketIndicesBuffer.Length = bucketResolutionSq;
            for (int i = 0; i < bucketResolutionSq; ++i)
            {
                var bucketEntity = ecb.CreateEntity();
                var bucket = ecb.AddBuffer<ObstacleBucket>(bucketEntity);
                bucketIndicesBuffer[i] = bucketEntity;
            }

            ecb.Playback(EntityManager);
        }
    }

    protected override void OnUpdate()
    {
        if (skip-- > 0)
            return;
        Random rand = new Random(1234);
        var mapSize = GetComponent<MapSetting>(GetSingletonEntity<MapSetting>()).WorldSize;
        UnityEngine.Debug.Log($"MapSize: {mapSize}");
        Entity obstacleBucketEntity = GetSingletonEntity<ObstacleBucketEntity>();
        //using (var ecb = new EntityCommandBuffer(Allocator.Temp))
        //{
        //    Entities
        //        .WithStructuralChanges()
        //        .ForEach((Entity entity, in ObstacleSpawner spawner) =>
        //        {
        //            var bucketIndicesBuffer = GetBuffer<ObstacleBucketIndices>(obstacleBucketEntity);
        //            int bucketResolutionSq = bucketResolution * bucketResolution;
        //            bucketIndicesBuffer.Length = bucketResolutionSq;
        //            for (int i = 0; i < bucketResolutionSq; ++i)
        //            {
        //                var bucketEntity = ecb.CreateEntity();
        //                bucketIndicesBuffer[i] = bucketEntity;
        //                var bucket = ecb.AddBuffer<ObstacleBucket>(bucketEntity);
        //            }
        //        }).Run();
        //    ecb.Playback(EntityManager);
        //}

        using (var ecb = new EntityCommandBuffer(Allocator.Temp))
        {
            Entities
                //.WithStructuralChanges()
                .ForEach((Entity entity, in ObstacleSpawner spawner) =>
                {
                    // Add logic for creating the obstacle rings
                    ecb.DestroyEntity(entity);

                    var bucketIndicesBuffer = GetBuffer<ObstacleBucketIndices>(obstacleBucketEntity);

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
                                UnityEngine.Debug.Log("AFter Instantiate");

                                var translation = new Translation
                                {
                                    Value = new float3(x, y, 0)
                                };
                                ecb.SetComponent(obstacleEntity, translation);
                                ecb.AddComponent(obstacleEntity, new Scale
                                {
                                    Value = spawner.ObstacleRadius
                                });

                                var pos = new float2
                                {
                                    x = translation.Value.x,
                                    y = translation.Value.y
                                };
                                float radius = spawner.ObstacleRadius;

                                float posX = translation.Value.x;
                                float posY = translation.Value.y;

                                for (int xx = (int)((posX - radius) / mapSize * bucketResolution); xx <= (int)((posX + radius) / mapSize * bucketResolution); ++xx)
                                {
                                    if (xx < 0 || xx > bucketResolution)
                                    {
                                        UnityEngine.Debug.Log("Continue x");
                                        continue;
                                    }

                                    for (int yy = (int)((posY - radius) / mapSize * bucketResolution); yy <= (int)((posY + radius) / mapSize * bucketResolution); ++yy)
                                    {
                                        if (yy < 0 || yy > bucketResolution)
                                        {
                                            UnityEngine.Debug.Log("Continue y");
                                            continue;
                                        }
                                        //int xxx = (int)((float)xx / mapSize * (float)bucketResolution);
                                        //int yyy = (int)((float)yy / mapSize * (float)bucketResolution);
                                        int bucketEntityIndex = xx * bucketResolution + yy;

                                        UnityEngine.Debug.Log($"Start AddToBuffer to {xx} {yy} {bucketEntityIndex}");
                                        var bucketEntity = bucketIndicesBuffer[bucketEntityIndex];
                                        var buffer = GetBuffer<ObstacleBucket>(bucketEntity);
                                        buffer.Add(translation);
                                        //ecb.<ObstacleBucket>(bucketEntity, obstacleEntity);

                                        //buffer.Add(obstacleEntity);
                                        UnityEngine.Debug.Log("End AddToBuffer");

                                        //var bucket = GetBuffer<ObstacleBucket>(bucketEntity);
                                        //bucket.Add(translation);
                                        //ecb.AppendToBuffer<ObstacleBucket>(bucketEntity, translation);
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
