using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = System.Random;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeMovementSystem : SystemBase
    {
        private EntityQuery resourceQuery;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            RequireSingletonForUpdate<ListSingelton>();

            Entities.WithAll<Bee>().ForEach((ref BeeMovement beeMovement, in Translation translation) =>
            {
                // Initialize the smooth position with a small offset
                beeMovement.SmoothPosition = translation.Value + new float3(0.1f, 0f, 0f);
            }).ScheduleParallel();
        }

        protected override void OnUpdate()
        {
            float deltaTime = World.Time.DeltaTime;
            float3 currentTargetPosition = float3.zero;

            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            NativeList<Entity> beeEntities = new NativeList<Entity>(Allocator.TempJob);
            
            Entities.WithAll<Bee>().ForEach((Entity entity) =>
            {
                beeEntities.Add(entity);
            }).Run();
            
            Debug.Log("Added entities: " + beeEntities.Length);

            Entities.WithAll<Bee>().WithNativeDisableContainerSafetyRestriction(allTranslations).WithNativeDisableContainerSafetyRestriction(beeEntities).ForEach(
                (Entity entity, ref Translation translation, ref Rotation rotation, ref BeeMovement beeMovement,
                    ref BeeTargets beeTargets, ref IsHoldingResource isHoldingResource, ref HeldResource heldResource) =>
                {
                    if (isHoldingResource.Value)
                    {
                        // Switch target to home if holding a resource
                        currentTargetPosition = beeTargets.HomePosition;
                    }
                    else if (beeTargets.ResourceTarget != Entity.Null)
                    {
                        // If a resource target is assigned to the current bee select it as the current target
                        // (if not holding a resource => bee is home => go for a new resource)
                        currentTargetPosition = allTranslations[beeTargets.ResourceTarget].Value;
                    }
                    
                    float3 delta = currentTargetPosition - translation.Value;
                    float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);

                    if (distanceFromTarget < beeTargets.TargetReach) // Target reached
                    {
                        if (!isHoldingResource.Value)
                        {
                            // Not holding a resource and reached a target resource
                            isHoldingResource.Value = true;
                            isHoldingResource.JustPickedUp = true;
                        }
                        else
                        {
                            // Holding a resource and reached home
                            beeTargets.ResourceTarget = Entity.Null;
                            isHoldingResource.Value = false;
                        }
                    }

                    // Add velocity towards the current target
                    beeMovement.Velocity += delta * (beeMovement.ChaseForce / distanceFromTarget);
                    
                    // Add random jitter
                    float3 randomJitter = beeTargets.random.NextFloat3(-1f, 1f);
                    beeMovement.Velocity += randomJitter * beeMovement.FlightJitter;
                    
                    // Apply damping (also limits velocity so that it does not keep increasing indefinitely)
                    beeMovement.Velocity *= 1f - beeMovement.Damping;
                    
                    // 1. Get all bee entities
                    // 2. Choose a random bee entity
                    // 3. Get a bee's position from "allTranslations"

                    if (beeEntities.Length > 0)
                    {
                        Debug.Log("Not zero");
                        int randomBeeIndex = beeTargets.random.NextInt(beeEntities.Length);
                        Debug.Log("Random bee index: " + randomBeeIndex);
                        Entity randomBee = beeEntities.ElementAt(randomBeeIndex);
                        float3 randomBeePosition = allTranslations[randomBee].Value;
                        float3 beeDelta = randomBeePosition - translation.Value;
                        float beeDistance = math.sqrt(beeDelta.x * beeDelta.x + beeDelta.y * beeDelta.y + beeDelta.z * beeDelta.z);

                        if (beeDistance > 0f)
                        {
                            beeMovement.Velocity += beeDelta * (beeMovement.TeamAttraction / beeDistance);
                        }
                    }
                    
                    // List<Bee> allies = teamsOfBees[bee.team];
                    // Bee attractiveFriend = allies[Random.Range(0,allies.Count)]; // Randomly choose an ally bee
                    // Vector3 delta = attractiveFriend.position - bee.position; // distance between this bee and the ally
                    // float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z); // The Pythagorean theorem extended to 3D
                    // if (dist > 0f) {
                    //     bee.velocity += delta * (teamAttraction * deltaTime / dist); // Add velocity towards an ally
                    // }

                    // Move bee closer to the target
                    translation.Value += beeMovement.Velocity * deltaTime;

                    // Face the direction of flight with smoothing
                    float3 oldSmoothPosition = beeMovement.SmoothPosition;
                    beeMovement.SmoothPosition = math.lerp(beeMovement.SmoothPosition, translation.Value,deltaTime * beeMovement.RotationStiffness);
                    float3 smoothDirection = beeMovement.SmoothPosition - oldSmoothPosition;
                    rotation.Value = quaternion.LookRotation(smoothDirection, new float3(0,1,0));
                }).ScheduleParallel();

            beeEntities.Dispose();
        }
    }
}