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
        }

        protected override void OnUpdate()
        {
            float deltaTime = World.Time.DeltaTime;
            float3 currentTargetPosition = float3.zero;

            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            // var buffer = GetBuffer<BeeResourcePair>(GetSingletonEntity<BufferSingelton>());

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
                    beeMovement.Velocity += delta * (beeMovement.ChaseForce * deltaTime / distanceFromTarget);
                    // Apply damping (also limits velocity so that it does not keep increasing indefinitely)
                    beeMovement.Velocity *= 1f - beeMovement.Damping;

                    // Move bee closer to the target
                    translation.Value += beeMovement.Velocity * deltaTime;
                }).ScheduleParallel();
        }
    }
}