using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;

public class AntHomeSteeringSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity homeEntity = GetSingletonEntity<Home>();
        Translation homeTranslation= EntityManager.GetComponentData<Translation>(homeEntity);
        float2 homePos = new float2(homeTranslation.Value.x, homeTranslation.Value.y);
        
        Entity antMovementParametersEntity = GetSingletonEntity<AntMovementParameters>();
        AntMovementParameters antMovementParameters =
            EntityManager.GetComponentData<AntMovementParameters>(antMovementParametersEntity);
        
        float homeSteerWeight = antMovementParameters.homeWeight;
        float originalDirectionWeight = 1.0f - homeSteerWeight;
        
        Entities
            .WithAll<Ant, CanSeeHome, HasFood>()
            .ForEach((ref Heading heading, in Translation translation) =>
            {
                float2 antPos = new Vector2(translation.Value.x, translation.Value.y);
                float2 normalizedHomeDirection = math.normalize(homePos - antPos);
                heading.heading = math.normalize(heading.heading * originalDirectionWeight) + (normalizedHomeDirection * homeSteerWeight);
            }).ScheduleParallel();
    }
}