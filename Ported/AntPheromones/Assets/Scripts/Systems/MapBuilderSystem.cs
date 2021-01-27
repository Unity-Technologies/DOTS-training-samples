using System;

using UnityEngine;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class MapBuilderSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Map>();
    }

    protected override void OnUpdate()
    {
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)math.max(DateTime.Now.Millisecond, 1));

        Entity mapEntity = GetSingletonEntity<Map>();
        Map map = GetComponent<Map>(mapEntity);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        Entities.WithAll<MapBuilder>().ForEach((Entity entity) =>
        {
            ecb.RemoveComponent<MapBuilder>(entity);

            for (int n = 1; n <= map.numberOfRings; ++n) // start:1 because center (0) is home
            {
                float radius = (float)n / (map.numberOfRings + 1.0F);
                float x = radius * map.dimensions.x * 0.5F;
                float y = radius * map.dimensions.y * 0.5F;
                radius = (x + y) / 2.0F; // There's probably a more correct way to do this line.

                float circumference = radius * 2.0F * math.PI;
                int count = Mathf.CeilToInt(circumference / (2.0F * map.obstacleRadius) * 2.0F);

                float startingAngle = rand.NextInt(count);

                int openings = rand.NextInt(2) + 1;

                for (int i = 0; i < count; ++i)
                {
                    float percent = (float)i / count;

                    if ((percent * openings) % 1.0F < 0.8F)
                    {
                        float yaw = ((float)i + startingAngle) / count * math.PI * 2.0F;

                        Entity obstacle = ecb.Instantiate(map.obstaclePrefab);

                        Translation translation = new Translation
                        {
                            Value = new float3(math.cos(yaw) * x, math.sin(yaw) * y, 0)
                        };

                        Obstacle obstacleComponentData = new Obstacle();

                        ecb.SetComponent<Translation>(obstacle, translation);
                        ecb.AddComponent<Obstacle>(obstacle, obstacleComponentData);
                        ecb.AddComponent<URPMaterialPropertyBaseColor>(obstacle, new URPMaterialPropertyBaseColor { Value = map.obstacleColor });
                    }
                }
            }

            Entity food = ecb.Instantiate(map.foodPrefab);

            float foodAngle = rand.NextFloat() * 2.0F * math.PI;
            map.foodLocation = new float2(math.cos(foodAngle) * map.dimensions.x * 0.475F,
                                          math.sin(foodAngle) * map.dimensions.y * 0.475F);

            Translation foodTranslation = new Translation { Value = new float3(map.foodLocation.x, map.foodLocation.y, 0) };
            ecb.SetComponent<Translation>(food, foodTranslation);
            ecb.AddComponent<URPMaterialPropertyBaseColor>(food, new URPMaterialPropertyBaseColor
            {
                Value = map.foodColor
            });

            Entity home = ecb.Instantiate(map.homePrefab);
            Translation homeTranslation = new Translation { Value = new float3() };
            ecb.SetComponent<Translation>(home, homeTranslation);
            ecb.AddComponent<URPMaterialPropertyBaseColor>(home, new URPMaterialPropertyBaseColor
            {
                Value = map.homeColor
            });
        }).Run();

        ecb.Playback(EntityManager);
    }
}
