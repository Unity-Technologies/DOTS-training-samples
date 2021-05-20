using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class WallSpawnerSystem : SystemBase
{
    private EntityQuery wallSpawnerQuery;
    
    protected override void OnCreate()
    {
        RequireForUpdate(wallSpawnerQuery);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var screenSize = GetSingleton<ScreenSize>();
        var wallSpawner = GetSingleton<WallSpawner>();
        const float degToRad = (math.PI * 2) / 360;

        for (int i = 0; i < wallSpawner.wallCount; i++)
        {
            int wallExitCount = UnityRandom.Range(1, 3);
            float openingSize = UnityRandom.Range(20.0f, 40.0f);
            float openingAngle = UnityRandom.Range(0.0f, 360.0f);
            
            float wallSpacing = (float)(screenSize.Value / 2) / (wallSpawner.wallCount + 1) * (i + 1);

            var openings = new NativeArray<float2>(wallExitCount, Allocator.TempJob);
            for (int j = 0; j < openings.Length; j++)
            {
                float startAngle = openingAngle - openingSize + (180 * j);
                
                startAngle = startAngle < 0 ? startAngle + 360 : startAngle % 360;
                
                float endAngle = openingAngle + openingSize + (180 * j);
                endAngle %= 360;

                openings[j] = new float2(startAngle, endAngle);
            }
            
            Entities
                .WithStoreEntityQueryInField(ref wallSpawnerQuery)
                .ForEach((Entity entity, in WallSpawner WallSpawner, in Respawn respawn) =>
                {
                    ecb.RemoveComponent<Respawn>(entity);
                    
                    for (int j = 0; j < openings.Length; j++)
                    {
                        var wallEntity = ecb.CreateEntity();
                        ecb.AddComponent<Wall>(wallEntity, new Wall
                        {
                            Angles = openings[j],
                            Radius = wallSpacing
                        });
                    }
                    
                    for (int a = 0; a < 360; a += (wallSpawner.wallCount * 2) / (i + 1))
                    {
                        bool isOpening = false;
                        for (int j = 0; j < openings.Length; j++)
                        {
                            if (openings[j].x < openings[j].y)
                            {
                                if (a > openings[j].x && a < openings[j].y)
                                    isOpening = true;
                            }
                            else
                            {
                                if (a > openings[j].x || a < openings[j].y)
                                    isOpening = true;
                            }
                        }
                        if (isOpening) continue;
                        
                        var wallSegment = ecb.Instantiate(wallSpawner.wallPrefab);
                        ecb.AddComponent<WallSegment>(wallSegment);
                        float2 segmentPoint = new float2(math.cos(a * degToRad), math.sin(a * degToRad));
                        segmentPoint *= wallSpacing;
                        ecb.SetComponent<Translation>(wallSegment, new Translation
                        {
                             Value = new float3(segmentPoint.x, segmentPoint.y, 0)
                        });
                        ecb.SetComponent<NonUniformScale>(wallSegment, new NonUniformScale()
                        {
                            Value = new float3(3, 3, 3)
                        });
                    }
                }).Run();

            openings.Dispose();
        }
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}