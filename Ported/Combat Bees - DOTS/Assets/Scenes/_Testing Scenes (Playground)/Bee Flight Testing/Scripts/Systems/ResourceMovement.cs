using CombatBees.Testing.BeeFlight;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace CombatBees.Testing.BeeFlight
{
    public partial class ResourceMovement : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
        }
        
        protected override void OnUpdate()
        {
            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            var allHeldResources = GetComponentDataFromEntity<HeldResource>(true);
            
            // Bee - HeldResource, Translation
            
            // 1) set held resource in the be movement system
            // 2) in entities for each in the resource movement system, we want to check if any bee is holding the current resource
            
            //NativeArray<Entity> allEntities = EntityManager.GetAllEntities(Allocator.TempJob);
            
            bool beeIsHoldingResource = false;
            Entity beeResourceTarget = Entity.Null;
            
            Entities.WithAll<Bee>().ForEach((in IsHoldingResource isHoldingResource, in BeeTargets beeTargets) =>
            {
                beeIsHoldingResource = isHoldingResource.Value;
                beeResourceTarget = beeTargets.ResourceTarget;
            }).Run();

            Entities.WithAll<Resource>().ForEach(
                (Entity entity, ref Translation translation, ref Holder holder) =>
                {
                    // iterate over all bees and check if any of them has "HeldResource" == entity (current resource)
                    //
                    // foreach (Entity globalEntity in allEntities)
                    // {
                    //     //var heldResource = EntityManager.GetComponentData<HeldResource>(globalEntity);
                    //     var heldResource = allHeldResources[globalEntity];
                    //     
                    //     
                    //     if (heldResource.Value == entity)
                    //     {
                    //         Debug.Log("We fucking found it!");
                    //     }
                    // }
                    //
                    // Debug.Log("Is held" + allHeldResources.HasComponent(entity));

                    // Debug.Log("RESOURCE HOLDER: " + holder.Value); // OK
                    // Debug.Log("TEST NUM: " + holder.TestNumber);
                    //
                    if (holder.Value != Entity.Null && beeIsHoldingResource && entity == beeResourceTarget)
                    {
                        Debug.Log("Holder is NOT NULL"); // doesn't trigger
                        translation.Value = allTranslations[holder.Value].Value;
                    }
                }).Run();
            
        }
    }
}