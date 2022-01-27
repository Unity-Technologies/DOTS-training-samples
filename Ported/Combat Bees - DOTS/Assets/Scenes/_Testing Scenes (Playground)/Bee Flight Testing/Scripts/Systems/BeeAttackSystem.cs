using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeAttackSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            RequireSingletonForUpdate<ListSingelton>();
        }

        protected override void OnUpdate()
        {        
            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            float deltaTime = World.Time.DeltaTime;
            float3 currentTargetPosition = float3.zero;
            NativeList<Entity> killedBees = new NativeList<Entity>(Allocator.TempJob);

            
            Entities.WithAll<Bee>().WithNativeDisableContainerSafetyRestriction(allTranslations).ForEach(
                (Entity entity, ref Translation translation, ref Rotation rotation, ref BeeMovement beeMovement, ref Bee bee,
                    ref BeeTargets beeTargets, ref IsHoldingResource isHoldingResource, ref HeldResource heldResource) =>
                {
                    if (beeTargets.AttackTarget != Entity.Null)
                    {
                        // If a enemy target is assigned to the current bee select it as the current target
                        currentTargetPosition = allTranslations[beeTargets.AttackTarget].Value;
                    }
                    
                    float3 delta = currentTargetPosition - translation.Value;
                    float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                    
                    if (beeTargets.AttackTarget != Entity.Null && distanceFromTarget < beeTargets.TargetReach) // Target reached
                    {
                        killedBees.Add(beeTargets.AttackTarget); // Add to the list used in the next step
                    }
                }).Run();
            
            Entities.WithAll<Bee>().ForEach((Entity entity, ref Bee bee) =>
            {
                bool killed = false;
                
                foreach (var killedBee in killedBees)
                {
                    if (entity == killedBee) killed = true; // The resource been assigned to a bee
                }
                
                if (killed)
                {
                    bee.dead = true; // Mark the resource as not available
                }
            }).Run();
        }
    }
}