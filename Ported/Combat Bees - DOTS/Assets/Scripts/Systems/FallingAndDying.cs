using System;
using Combatbees.Testing.Maria;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class FallingAndDying : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }

    protected override void OnUpdate()
    {
        float3 containerMinPos = GetSingleton<Container>().MinPosition;
        float3 containerMaxPos = GetSingleton<Container>().MaxPosition;
        var deltaTime = Time.DeltaTime;
        
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities.WithAll<Falling>().ForEach((Entity entity,ref Translation translation ,ref Velocity velocity,ref Falling falling, ref HeldItem heldItem) => {
            try
            {
                if (falling.shouldFall)
                {
                    float3 newPosition = translation.Value + velocity.Value * deltaTime;
        
                    // Clamp the position to be within the container
                    float3 clampedPosition = math.clamp(newPosition, containerMinPos, containerMaxPos);
        
                    // If the resource hits the floor reset its velocity
                    if (clampedPosition.y > newPosition.y) velocity.Value = float3.zero;
        
                    translation.Value = clampedPosition;
        
                    if (0 <= falling.timeToLive)
                    {
                        falling.timeToLive -= deltaTime;
                    }
                    else
                    {
                        // BUG: not really working?
                        if (heldItem.Value != Entity.Null)
                        {
                            // also set the resource that the bee is holding again to a free resouce
                            var targetedComponent = GetComponent<Targeted>(heldItem.Value);
                            targetedComponent.Value = false;
                            SetComponent(heldItem.Value, targetedComponent);
                        }
                        ecb.DestroyEntity(entity); // Destroy the dead bee - MUST BE as last
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"null?: falling: {falling}, entity: {entity}, heldItem: {heldItem}");
                throw e;
            }
            
        }).Run();
     
        // ecb.Playback(EntityManager);
        // ecb.Dispose();
        
        Entities.WithAll<BloodTag>().ForEach((Entity entity,ref Translation translation ,ref Velocity velocity,ref Falling falling) => {
        
            if (falling.shouldFall)
            {
                float3 newPosition = translation.Value + velocity.Value * deltaTime;
        
                // Clamp the position to be within the container
                float3 clampedPosition = math.clamp(newPosition, containerMinPos, containerMaxPos);
        
                // If the resource hits the floor reset its velocity
                if (clampedPosition.y > newPosition.y) velocity.Value = float3.zero;
        
                translation.Value = clampedPosition;
        
                if (0 <= falling.timeToLive)
                {
                    falling.timeToLive -= deltaTime;
                }
                else
                {
                    ecb.DestroyEntity(entity); // Destroy the dead bee - MUST BE as last
                }
            }
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
