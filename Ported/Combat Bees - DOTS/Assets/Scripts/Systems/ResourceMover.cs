using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class ResourceMover : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }

    protected override void OnUpdate()
    {
        float3 containerMinPos = GetSingleton<Container>().MinPosition;
        float3 containerMaxPos = GetSingleton<Container>().MaxPosition;

        float deltaTime = Time.DeltaTime;
        var allTranslations = GetComponentDataFromEntity<Translation>(true);
        var allVelocities = GetComponentDataFromEntity<Velocity>(true);

        Entities.WithAll<ResourceTag>().WithNativeDisableContainerSafetyRestriction(allTranslations)
            // .WithNativeDisableContainerSafetyRestriction(allVelocities)
            .ForEach((ref Translation translation, ref Velocity velocity, ref Holder holder) =>
            {
                if (holder.Value == Entity.Null) // if no holder, apply gravity
                    {
                        float3 newPosition = translation.Value + velocity.Value * deltaTime;

                        // Clamp the position to be within the container
                        float3 clampedPosition = math.clamp(newPosition, containerMinPos, containerMaxPos);

                        // If the resource hits the floor reset its velocity
                        if (clampedPosition.y > newPosition.y) velocity.Value = float3.zero;

                        translation.Value = clampedPosition;
                    }
                    else // if holder, follow his position + offset
                    {
                        translation.Value = allTranslations[holder.Value].Value;
                        // Debug.Log("The holder of the bee died, resetting");
                        // holder.Value = Entity.Null;
                            
                        // Comment out the line below to make resources drop straight to the ground
                        // velocity.Value = allVelocities[holder.Value].Value;
                    }
                
            }).ScheduleParallel();
    }
}