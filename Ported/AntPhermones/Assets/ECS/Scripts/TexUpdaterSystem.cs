using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class TexUpdaterSystem : SystemBase
{
    const float dt = 1.0f / 60;
    const float randomSteering = 0.1f;
    const float decay = 0.999f;
    const float trailAddSpeed = 0.3f;
    const float excitementWhenLookingForFood = 4.0f;
    const float excitementWhenGoingBackToNest = 5.0f;

    Entity textureSingleton; //Use a singleton entity for the buffer entity

    Color[] colors = new Color[RefsAuthoring.TexSize * RefsAuthoring.TexSize];

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TexSingleton>();
    }

    static void DropPheromones(float x, float y, Vector2 bounds, NativeArray<PheromonesBufferData> localPheromones, float speed, float dt, int TexSize, float excitement)
    {
        Vector2 texelCoord = new Vector2(0.5f * (-x / bounds.x) + 0.5f, 0.5f * (-y / bounds.y) + 0.5f);
        int index = (int)(texelCoord.y * TexSize) * TexSize + (int)(texelCoord.x * TexSize);
        localPheromones[index] += (speed * dt * excitement) * (1.0f - localPheromones[index]);
        localPheromones[index] = ((float)localPheromones[index] > 1.0f) ? 1.0f : (float)localPheromones[index];
    }
    
    protected override void OnUpdate()
    {
        int TexSize = RefsAuthoring.TexSize;

        //Get the pheremones data 
        EntityQuery doesTextInitExist = GetEntityQuery(typeof(TexInitialiser));
        if(!doesTextInitExist.IsEmpty)
        {
            return;
        }

        Vector2 bounds = AntMovementSystem.bounds;
        var localPheromones = EntityManager.GetBuffer<PheromonesBufferData>(GetSingletonEntity<TexSingleton>());
        
        Entities
            .WithNativeDisableParallelForRestriction(localPheromones)
            .ForEach((int entityInQueryIndex, ref Translation translation, ref Direction direction, ref RandState rand, in Speed speed, in AntLookingForFood dummy) =>
            {
                DropPheromones(translation.Value.x, translation.Value.z, bounds, localPheromones.AsNativeArray(), speed.Value, dt, TexSize, excitementWhenLookingForFood);
               
            })
            .ScheduleParallel();

        Entities
            .WithNativeDisableParallelForRestriction(localPheromones)
            .ForEach((int entityInQueryIndex, ref Translation translation, ref Direction direction, ref RandState rand, in Speed speed, in AntLookingForNest dummy) =>
            {
                DropPheromones(translation.Value.x, translation.Value.z, bounds, localPheromones.AsNativeArray(), speed.Value, dt, TexSize, excitementWhenGoingBackToNest);
            })
            .ScheduleParallel();

        Entities
            .ForEach((Refs map) =>
            {
                for (int i = 0; i < TexSize; ++i)
                {
                    for (int j = 0; j < TexSize; ++j)
                    {
                        colors[j * TexSize + i].r = localPheromones[j * TexSize + i]; ;
                        colors[j * TexSize + i].g = 0;
                        colors[j * TexSize + i].b = 0;
                    }
                }
                map.PheromoneMap.SetPixels(0, 0, TexSize, TexSize, colors);
                map.PheromoneMap.Apply();
            })
            .WithoutBurst()
            .Run();
        
    }

    
    
}
