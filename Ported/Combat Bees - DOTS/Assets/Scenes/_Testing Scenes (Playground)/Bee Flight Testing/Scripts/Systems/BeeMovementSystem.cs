using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Debug = UnityEngine.Debug;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeMovementSystem : SystemBase
    {
        private EntityQuery resourceQuery;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
        }

        protected override void OnUpdate()
        {
            float deltaTime = World.Time.DeltaTime;
            float3 currentTarget = float3.zero;

            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            var allHolders = GetComponentDataFromEntity<Holder>();

            Entities.WithAll<Bee>().WithNativeDisableContainerSafetyRestriction(allTranslations).ForEach(
                (Entity entity, ref Translation translation, ref Rotation rotation, ref BeeMovement beeMovement,
                    ref BeeTargets beeTargets, ref IsHoldingResource isHoldingResource) =>
                {
                    // float3 delta;
                    // float distanceFromTarget;

                    if (isHoldingResource.Value)
                    {
                        currentTarget = beeTargets.HomeTarget;
                        // delta = currentTarget - translation.Value;
                        // distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                    }
                    else if (beeTargets.ResourceTarget != Entity.Null)
                    {
                        currentTarget = allTranslations[beeTargets.ResourceTarget].Value;
                    }

                    //float3 targetPos = allTranslations[beeTargets.ResourceTarget].Value;
                    float3 delta = currentTarget - translation.Value;
                    float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);

                    if (distanceFromTarget < beeTargets.TargetReach)
                    {
                        if (!isHoldingResource.Value)
                        {
                            Holder targetResourceHolder = allHolders[beeTargets.ResourceTarget];
                            targetResourceHolder.Value = entity;
                            isHoldingResource.Value = true;
                        }
                        else
                        {
                            Holder targetResourceHolder = allHolders[beeTargets.ResourceTarget];
                            targetResourceHolder.Value = Entity.Null;
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
                }).Schedule();

            //resourceChunks.Dispose();
        }
    }
}