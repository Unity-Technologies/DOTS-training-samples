using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;

public class AntFoodSteeringSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity goalEntity = GetSingletonEntity<Goal>();
        Translation goalTranslation= EntityManager.GetComponentData<Translation>(goalEntity);
        float2 goalPos = new float2(goalTranslation.Value.x, goalTranslation.Value.y);

        Entity antMovementParametersEntity = GetSingletonEntity<AntMovementParameters>();
        AntMovementParameters antMovementParameters =
            EntityManager.GetComponentData<AntMovementParameters>(antMovementParametersEntity);

        float goalSteerWeight = antMovementParameters.goalWeight;
        float originalDirectionWeight = 1.0f - goalSteerWeight;
        
        Entities
            .WithAll<Ant, CanSeeFood>()
            .WithNone<HasFood>()
            .ForEach((ref Heading heading, in Translation translation) =>
            {
                float2 antPos = new Vector2(translation.Value.x, translation.Value.y);
                float2 normalizedGoalDirection = math.normalize(goalPos - antPos);
                heading.heading = math.normalize(heading.heading * originalDirectionWeight) + (normalizedGoalDirection * goalSteerWeight);
            }).ScheduleParallel();
    }
}
