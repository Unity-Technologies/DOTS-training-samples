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

            Entity ringEntity = ecb.CreateEntity();
            DynamicBuffer<RingElement> rings = ecb.AddBuffer<RingElement>(ringEntity);
            rings.Length = map.numberOfRings;

            for (int n = 1; n <= map.numberOfRings; ++n) // start:1 because center (0) is home
            {
                RingElement ring = rings[n - 1];

                ring.offsets.x = (float)n / (map.numberOfRings + 1.0F) * map.dimensions.x * 0.5F;
                ring.offsets.y = (float)n / (map.numberOfRings + 1.0F) * map.dimensions.y * 0.5F;

                ring.halfThickness = map.obstacleRadius;

                float startingAngle = rand.NextFloat() * math.PI * 2.0F;
                float openingRadians = math.radians(map.openingDegrees);

                ring.numberOfOpenings = rand.NextInt(2) + 1;
                switch (ring.numberOfOpenings)
                {
                    case 1:
                        {
                            float endAngle = startingAngle + openingRadians;
                            endAngle = (endAngle) % (math.PI * 2.0F);
                            ring.opening0.angles = new float2(startingAngle, endAngle);
                        }
                        break;
                    case 2:
                        {
                            float endAngle = startingAngle + openingRadians;
                            endAngle = (endAngle) % (math.PI * 2.0F);
                            ring.opening0.angles = new float2(startingAngle, startingAngle + openingRadians);
                            startingAngle = (startingAngle + math.PI) % (math.PI * 2.0F);
                            endAngle = startingAngle + openingRadians;
                            endAngle = (endAngle) % (math.PI * 2.0F);
                            ring.opening1.angles = new float2(startingAngle, startingAngle + openingRadians);
                        }
                        break;
                }
                rings[n - 1] = ring;

                float C = math.length(ring.offsets) * math.PI * 2.0F;
                int count = (int)math.ceil(C / (2.0F * map.obstacleRadius) * 2.0F);

                for (int i = 0; i < count; ++i)
                {
                    float yaw = math.saturate(((float)i + 1.0F) / (float)count) * math.PI * 2.0F;

                    bool spawn = true;
                    switch (ring.numberOfOpenings)
                    {
                        case 1:
                            {
                                spawn = !MapManagementSystem.IsBetween(ring.opening0.angles, yaw);
                            }
                            break;
                        case 2:
                            {
                                spawn = !MapManagementSystem.IsBetween(ring.opening0.angles, yaw);
                                if (spawn)
                                    spawn = !MapManagementSystem.IsBetween(ring.opening1.angles, yaw);
                            }
                            break;
                    }

                    if (spawn)
                    {
                        Entity obstacle = ecb.Instantiate(map.obstaclePrefab);
                        float x = math.cos(yaw);
                        float y = math.sin(yaw);
                        Translation translation = new Translation { Value = new float3(math.cos(yaw) * ring.offsets.x, math.sin(yaw) * ring.offsets.y, 0) };

                        ecb.SetComponent<Translation>(obstacle, translation);
                        ecb.AddComponent<Obstacle>(obstacle, new Obstacle());
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
