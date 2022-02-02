using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
            NativeList<Entity> beeAEntities = new NativeList<Entity>(Allocator.TempJob);
            NativeList<Entity> beeBEntities = new NativeList<Entity>(Allocator.TempJob);
            Entities.WithAll<Bee>().ForEach((Entity entity, in BeeMovement beeMovement) =>
            {
                if(beeMovement.TeamA)
                    beeAEntities.Add(entity);
                else
                    beeBEntities.Add(entity);
            }).Run();
            
            // Debug.Log("Added entities: " + beeEntities.Length);

            Entities.WithAll<Bee>().WithNativeDisableContainerSafetyRestriction(allTranslations).ForEach(
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
                    //float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                    float distanceFromTarget = delta.DistanceToFloat();

                    if (distanceFromTarget < beeTargets.TargetReach) // Target reached
                    {
                        if (!isHoldingResource.Value)
                        {
                            // Not holding a resource and reached a target resource
                            isHoldingResource.Value = true;
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
                    
                    // Attraction to a random bee
                    Entity randomBee;
                    
                    if (beeMovement.TeamA)
                    {
                        int randomBeeIndex = beeTargets.random.NextInt(beeAEntities.Length);
                        randomBee = beeAEntities.ElementAt(randomBeeIndex);
                            
                    }
                    else
                    {
                        int randomBeeIndex = beeTargets.random.NextInt(beeBEntities.Length);
                        randomBee = beeBEntities.ElementAt(randomBeeIndex);
                    }
                    
                    float3 randomBeePosition = allTranslations[randomBee].Value;
                    float3 beeDelta = randomBeePosition - translation.Value;
                    //float beeDistance = math.sqrt(beeDelta.x * beeDelta.x + beeDelta.y * beeDelta.y + beeDelta.z * beeDelta.z);
                    float beeDistance = beeDelta.DistanceToFloat();
                    if (beeDistance > 0f)
                    { 
                        beeMovement.Velocity += beeDelta * (beeMovement.TeamAttraction / beeDistance);
                    }
                    
                    // Move bee closer to the target
                    translation.Value += beeMovement.Velocity * deltaTime;

                    // Face the direction of flight with smoothing
                    float3 oldSmoothPosition = beeMovement.SmoothPosition;
                    beeMovement.SmoothPosition = math.lerp(beeMovement.SmoothPosition, translation.Value,deltaTime * beeMovement.RotationStiffness);
                    float3 smoothDirection = beeMovement.SmoothPosition - oldSmoothPosition;
                    rotation.Value = quaternion.LookRotation(smoothDirection, new float3(0,1,0));
                }).Run(); // Why is beeEntities empty when Scheduled parallel?

            
            beeAEntities.Dispose();
            beeBEntities.Dispose();
        }
    }
}