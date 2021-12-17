using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Debug = UnityEngine.Debug;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeMovementSystem : SystemBase
    {
        private EntityQuery resourceQuery;
        [NativeDisableParallelForRestriction]
        public ComponentDataFromEntity<Holder> allHolders;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
        }

        protected override void OnUpdate()
        {
            float deltaTime = World.Time.DeltaTime;
            float3 currentTarget = float3.zero;

            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            allHolders = GetComponentDataFromEntity<Holder>(false);
            //var allBeeTransforms = GetComponent<Translation>(al);
            // NativeList<UnsafeList<Entity>> pairs = new NativeList<UnsafeList<Entity>>();
            var buffer = GetBuffer<BeeResourcePair>(GetSingletonEntity<BufferSingelton>());

            Entities.WithAll<Bee>().WithNativeDisableContainerSafetyRestriction(allTranslations).ForEach(
                (Entity entity, ref Translation translation, ref Rotation rotation, ref BeeMovement beeMovement,
                    ref BeeTargets beeTargets, ref IsHoldingResource isHoldingResource, ref HeldResource heldResource) =>
                {
                    // float3 delta;
                    // float distanceFromTarget;

                    if (isHoldingResource.Value)
                    {
                        currentTarget = beeTargets.HomeTarget;
                    }
                    else if (beeTargets.ResourceTarget != Entity.Null)
                    {
                        currentTarget = allTranslations[beeTargets.ResourceTarget].Value;
                    }
                    
                    //float3 targetPos = allTranslations[beeTargets.ResourceTarget].Value;
                    float3 delta = currentTarget - translation.Value;
                    float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);

                    if (distanceFromTarget < beeTargets.TargetReach) // Target reached
                    {
                        if (!isHoldingResource.Value)
                        {
                            // Not holding a resource and reached a target resource
                            //heldResource.Value = beeTargets.ResourceTarget;
                            
                            //pairs.Add(new UnsafeList<Entity>() {entity, beeTargets.ResourceTarget});
                            
                            // Holder targetResourceHolder = allHolders[beeTargets.ResourceTarget];
                            // Debug.Log(targetResourceHolder.Value);
                            // targetResourceHolder.Value = entity;
                            // Debug.Log(targetResourceHolder.Value);
                            isHoldingResource.Value = true;
                            //
                            // targetResourceHolder.TestNumber = 777;
                        }
                        else
                        {
                            // Holding a resource and reached home
                            
                            // Holder targetResourceHolder = allHolders[beeTargets.ResourceTarget];
                            // targetResourceHolder.Value = Entity.Null;
                            beeTargets.ResourceTarget = Entity.Null;
                            isHoldingResource.Value = false;
                        }
                    }
                    else
                    {
                        // On the way
                        if (isHoldingResource.Value)
                        {
                            

                            //GetComponent<Translation>(beeTargets.ResourceTarget).Value = float3.zero;
                            //allTranslations[beeTargets.ResourceTarget].Value = float3.zero;
                            
                            // float3 heldResource = allTranslations[beeTargets.ResourceTarget].Value;
                            // heldResource = 
                        }
                    }

                    // Add velocity towards the current target
                    beeMovement.Velocity += delta * (beeMovement.ChaseForce * deltaTime / distanceFromTarget);
                    // Apply damping (also limits velocity so that it does not keep increasing indefinitely)
                    beeMovement.Velocity *= 1f - beeMovement.Damping;

                    // Move bee closer to the target
                    translation.Value += beeMovement.Velocity * deltaTime;
                }).WithoutBurst().Run();
            
            //Debug.Log(pairs.Length);

            // foreach (var pair in pairs)
            // {
            //     pair.Dispose();
            // }
            //
            // pairs.Dispose();
        }
    }
}