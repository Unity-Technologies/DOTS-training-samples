using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FoodBuilderSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<ObstacleBuilder>();
    }

    protected override void OnUpdate()
    {
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)math.max(DateTime.Now.Millisecond, 1));

        Entity obstacleEntity = GetSingletonEntity<ObstacleBuilder>();
        ObstacleBuilder obstacleBuilder = GetComponent<ObstacleBuilder>(obstacleEntity);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        Entities
            .WithAll<FoodBuilder>()
            .WithNone<Initialized>()
            .ForEach((Entity entity,ref FoodBuilder foodBuilder) =>
        {
            ecb.AddComponent<Initialized>(entity);

            for (int i = 0; i < foodBuilder.numFood; ++i) {
                Entity foodEntity = ecb.Instantiate(foodBuilder.foodPrefab);

                float foodAngle = rand.NextFloat() * 2.0F * math.PI;
                foodBuilder.foodLocation = new float2(math.cos(foodAngle) * obstacleBuilder.dimensions.x * 0.475F,
                    math.sin(foodAngle) * obstacleBuilder.dimensions.y * 0.475F);

                Translation foodTranslation = new Translation
                    {Value = new float3(foodBuilder.foodLocation.x, foodBuilder.foodLocation.y, 0)};
                ecb.SetComponent(foodEntity, foodTranslation);
                ecb.AddComponent(foodEntity, new URPMaterialPropertyBaseColor
                {
                    Value = foodBuilder.foodColor
                });
                ecb.AddComponent(foodEntity,new Food());
                
                //ecb.SetComponent(entity,foodBuilder);
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

