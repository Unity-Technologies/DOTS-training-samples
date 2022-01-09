using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeMovementSystemBuffer : SystemBase
         {
        private EntityQuery resourceQuery;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            RequireSingletonForUpdate<BufferSingelton>();
           
        }

        protected override void OnUpdate()
        {
            float deltaTime = World.Time.DeltaTime;
            float3 currentTarget = float3.zero;

            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            var PairBuffer = GetBuffer<BeeResourcePair>(GetSingletonEntity<BufferSingelton>());

            Entities.WithAll<Bee>().WithNativeDisableContainerSafetyRestriction(allTranslations).ForEach(
                (Entity entity, ref Translation translation, ref Rotation rotation, ref BeeMovement beeMovement,
                    ref BeeTargets beeTargets, ref IsHoldingResource isHoldingResource, ref HeldResource heldResource) =>
                {
                   
                    foreach (var pair in PairBuffer)
                    {
                        if (pair.BeeEntity == entity)
                        {
                            if (isHoldingResource.Value)
                            {
                                currentTarget = beeTargets.HomePosition;
                            }
                            else if(pair.ResourceEntity!=Entity.Null)
                            {
                                currentTarget = allTranslations[pair.ResourceEntity].Value;
                            }
                        }
                    }
                    
                    float3 delta = currentTarget - translation.Value;
                    float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);

                    if (distanceFromTarget < beeTargets.TargetReach) // Target reached could be home or resource 
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
                            isHoldingResource.ReachedHome = true;
                        }
                    }

                    // Add velocity towards the current target
                    beeMovement.Velocity += delta * (beeMovement.ChaseForce * deltaTime / distanceFromTarget);
                    // Apply damping (also limits velocity so that it does not keep increasing indefinitely)
                    beeMovement.Velocity *= 1f - beeMovement.Damping;

                    // Move bee closer to the target
                    translation.Value += beeMovement.Velocity * deltaTime;
                }).WithoutBurst().Run();
        }
    }
}