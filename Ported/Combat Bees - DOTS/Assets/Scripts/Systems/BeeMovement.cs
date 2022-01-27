using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class BeeMovement : SystemBase
{
    private EntityQuery resourceQuery;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();

        Entities.WithAll<BeeTag>().ForEach((ref SmoothPosition smoothPosition, in Translation translation) =>
        {
            // Initialize the smooth position with a small offset
            smoothPosition.Value = translation.Value + new float3(0.1f, 0f, 0f);
        }).ScheduleParallel();
    }

    protected override void OnUpdate()
    {
        float deltaTime = World.Time.DeltaTime;
        float3 currentTargetPosition = float3.zero;

        var allTranslations = GetComponentDataFromEntity<Translation>(true);

        int beeCount = 0;

        NativeList<Entity> beeAEntities = new NativeList<Entity>(Allocator.TempJob);
        NativeList<Entity> beeBEntities = new NativeList<Entity>(Allocator.TempJob);
        
        Entities.WithAll<BeeTag>().ForEach((Entity entity, in Team team) =>
        {
            beeCount++;
            
            if(team.Value == TeamName.A)
                beeAEntities.Add(entity);
            else if (team.Value == TeamName.B)
                beeBEntities.Add(entity);
        }).Run();

        BeeProperties beeProperties = GetBeeProperties();
        NativeHashMap<Entity, Entity> grabbedItemsAndHolders = new NativeHashMap<Entity, Entity>(beeCount, Allocator.TempJob);
        NativeHashMap<Entity, Entity> droppedItems = new NativeHashMap<Entity, Entity>(beeCount, Allocator.TempJob);
        
        float3 containerMinPos = GetSingleton<Container>().MinPosition;
        float3 containerMaxPos = GetSingleton<Container>().MaxPosition;

        Entities.WithAll<BeeTag>().WithNativeDisableContainerSafetyRestriction(allTranslations).
            WithDisposeOnCompletion(beeAEntities).
            WithDisposeOnCompletion(beeBEntities).ForEach(
            (Entity entity, ref Translation translation, ref Velocity velocity, ref BeeTargets beeTargets, ref HeldItem heldItem, ref RandomState randomState, ref BeeStatus beeStatus, in Team team) =>
            {
                if (heldItem.Value != Entity.Null)
                {
                    // Switch target to home if holding a resource
                    currentTargetPosition = beeTargets.HomePosition;
                    currentTargetPosition.z = translation.Value.z;
                }
                else if (beeTargets.ResourceTarget != Entity.Null)
                {
                    // If a resource target is assigned to the current bee select it as the current target
                    // (if not holding a resource => bee is home => go for a new resource)
                    currentTargetPosition = allTranslations[beeTargets.ResourceTarget].Value;
                }
                
                float3 delta = currentTargetPosition - translation.Value;
                float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
        
                if (distanceFromTarget < beeProperties.TargetReach) // Target reached
                {
                    if (heldItem.Value == Entity.Null)
                    {
                        // if not holding a resource and reached a target resource, grab the resource
                        heldItem.Value = beeTargets.ResourceTarget;
                        // Used to propagate the holder to the held item
                        grabbedItemsAndHolders.Add(heldItem.Value, entity);
                    }
                    else
                    {
                        // if holding a resource and reached home, reset target and held item
                        droppedItems.Add(heldItem.Value, Entity.Null);
                        beeTargets.ResourceTarget = Entity.Null;
                        heldItem.Value = Entity.Null;
                        beeStatus.Value = Status.Idle;
                    }
                }
        
                // Add velocity towards the current target
                velocity.Value += delta * (beeProperties.ChaseForce / distanceFromTarget);
                
                // Add random jitter
                float3 randomJitter = randomState.Random.NextFloat3(-1f, 1f);
                velocity.Value += randomJitter * beeProperties.FlightJitter;
                
                // Apply damping (also limits velocity so that it does not keep increasing indefinitely)
                velocity.Value *= 1f - beeProperties.Damping;
                
                // Attraction to a random bee
                Entity randomBee;
                
                if (team.Value == TeamName.A)
                {
                    int randomBeeIndex = randomState.Random.NextInt(beeAEntities.Length);
                    randomBee = beeAEntities.ElementAt(randomBeeIndex);
                        
                }
                else if (team.Value == TeamName.B)
                {
                    int randomBeeIndex = randomState.Random.NextInt(beeBEntities.Length);
                    randomBee = beeBEntities.ElementAt(randomBeeIndex);
                }
                else
                {
                    return;
                }
                
                float3 randomBeePosition = allTranslations[randomBee].Value;
                float3 beeDelta = randomBeePosition - translation.Value;
                float beeDistance = math.sqrt(beeDelta.x * beeDelta.x + beeDelta.y * beeDelta.y + beeDelta.z * beeDelta.z);
                
                if (beeDistance > 0f)
                { 
                    velocity.Value += beeDelta * (beeProperties.TeamAttraction / beeDistance);
                }
                
                // Move bee closer to the target
                translation.Value += velocity.Value * deltaTime;

                // Clamp the position within the field container
                translation.Value = math.clamp(translation.Value, containerMinPos, containerMaxPos);
            }).Schedule();

        Entities.WithAll<BeeTag>().ForEach((ref Rotation rotation, ref SmoothPosition smoothPosition,
            in Translation translation) =>
        {
            // Face the direction of flight with smoothing
            float3 oldSmoothPosition = smoothPosition.Value;
            smoothPosition.Value = math.lerp(smoothPosition.Value, translation.Value,deltaTime * beeProperties.RotationStiffness);
            float3 smoothDirection = smoothPosition.Value - oldSmoothPosition;
            rotation.Value = quaternion.LookRotation(smoothDirection, new float3(0,1,0));
        }).Run();

        Entities.ForEach((Entity entity, ref Holder holder) =>
        {
            // Go through all held items (resources)
            if (grabbedItemsAndHolders.ContainsKey(entity))
            {
                holder.Value = grabbedItemsAndHolders[entity];
            } else if (droppedItems.ContainsKey(entity))
            {
                holder.Value = Entity.Null;
            } 
        }).Run();

        grabbedItemsAndHolders.Dispose();
        droppedItems.Dispose();
    }

    private BeeProperties GetBeeProperties()
    {
        BeeProperties beeProps = new BeeProperties();
        
        Entities.ForEach((in BeeProperties beeProperties) =>
        {
            beeProps = beeProperties;
        }).Run();

        return beeProps;
    }
}