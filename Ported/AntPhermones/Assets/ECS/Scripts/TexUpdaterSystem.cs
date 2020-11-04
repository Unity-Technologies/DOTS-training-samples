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

    Entity textureSingleton; //Use a singleton entity for the buffer entity


    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TexSingleton>();
    }

    
    protected override void OnUpdate()
    {
        int TexSize = RefsAuthoring.TexSize;
        float excitement = 4.0f; //TODO : increase the excitement level when holding resource (so it should be a component)

        //Get the pheremones data 
        EntityQuery doesTextInitExist = GetEntityQuery(typeof(TexInitialiser));
        if(!doesTextInitExist.IsEmpty)
        {
            return;
        }


        var localPheromones = EntityManager.GetBuffer<PheromonesBufferData>(GetSingletonEntity<TexSingleton>());

        Entities
            .WithNativeDisableParallelForRestriction(localPheromones)
            .ForEach((int entityInQueryIndex, ref Translation translation, ref Direction direction, ref RandState rand, in Speed speed) =>
            {
                int index = TextureHelper.GetTextureArrayIndexFromTranslation(translation);
                localPheromones[index] += (speed.Value * dt * excitement) * (1.0f - localPheromones[index]);
                localPheromones[index] = ((float)localPheromones[index] > 1.0f) ? 1.0f : (float)localPheromones[index];
               
            })
            .ScheduleParallel();

        Entities
            .ForEach((Refs map) =>
            {
                for (int i = 0; i < TexSize; ++i)
                {
                    for (int j = 0; j < TexSize; ++j)
                    {
                        localPheromones[j * TexSize + i] *= decay;
                        map.PheromoneMap.SetPixel(i, j, new Color(localPheromones[j * TexSize + i], 0, 0));
                    }
                }
                map.PheromoneMap.Apply();
            })
            .WithoutBurst()
            .Run();

        localPheromones[0] = 20f;
        localPheromones[1] = 27f;
        
    }

    
    
}
