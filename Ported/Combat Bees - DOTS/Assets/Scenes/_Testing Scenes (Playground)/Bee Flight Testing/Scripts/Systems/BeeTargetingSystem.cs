using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeTargetingSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            RequireSingletonForUpdate<ListSingelton>();
        }

        protected override void OnUpdate()
        {
            // Create resource list
            NativeList<Entity> freeResources = new NativeList<Entity>(Allocator.TempJob);
            NativeList<Entity> assignedResources = new NativeList<Entity>(Allocator.TempJob);

            Entities.WithAll<Resource>().ForEach((Entity entity, in ResourceState resourceState) =>
            {
                if (resourceState.Free) freeResources.Add(entity); // Find free resources (not targeted or home)
            }).Run();
            
            // Create Team Lists
            NativeList<Entity> beeAEntities = new NativeList<Entity>(Allocator.TempJob);
            NativeList<Entity> beeBEntities = new NativeList<Entity>(Allocator.TempJob);
            Entities.WithAll<Bee>().ForEach((Entity entity, in Bee bee) =>
            {
                if(bee.TeamA)
                    beeAEntities.Add(entity);
                else
                    beeBEntities.Add(entity);
            }).Run();
            
            if (freeResources.Length > 0)
            {
                Entities.WithAll<Bee>().ForEach((ref BeeTargets beeTargets, ref Bee bee) =>
                {
                    if (beeTargets.ResourceTarget == Entity.Null &&
                        beeTargets.AttackTarget == Entity.Null) // if bee does not have a target
                    {
                        // Make the bee an attacker or a resource-getter (dependent on the agression)
                        if (beeTargets.AttackTarget == Entity.Null && beeTargets.ResourceTarget == Entity.Null)
                        {
                            float r = beeTargets.random.NextFloat();
                            if (bee.TeamA || r < beeTargets.Aggression)
                            {
                                // choose enemy target
                                NativeList<Entity> listToModify;
                                if (bee.TeamA)
                                {
                                    listToModify = beeBEntities;
                                }
                                else
                                {
                                    listToModify = beeAEntities;
                                }

                                int randomResourceIndex = beeTargets.random.NextInt(listToModify.Length);
                                beeTargets.AttackTarget = listToModify.ElementAt(randomResourceIndex);
                                listToModify.RemoveAt(
                                    randomResourceIndex); // Remove from the list of available resources
                                // assignedAttackers.Add(beeTargets.ResourceTarget); // Add to the list used in the next step?
                            }
                            else
                            {
                                // choose resource
                                // Assign a random resource
                                int randomResourceIndex = beeTargets.random.NextInt(freeResources.Length);
                                beeTargets.ResourceTarget = freeResources.ElementAt(randomResourceIndex);
                                freeResources.RemoveAt(
                                    randomResourceIndex); // Remove from the list of available resources
                                assignedResources.Add(beeTargets
                                    .ResourceTarget); // Add to the list used in the next step
                            }
                        }
                    }
                }).Run();
            }
            
            Entities.WithAll<Resource>().ForEach((Entity entity, ref ResourceState resourceState) =>
            {
                bool assigned = false;
                
                foreach (var assignedResource in assignedResources)
                {
                    if (entity == assignedResource) assigned = true; // The resource been assigned to a bee
                }
                
                if (assigned)
                {
                    resourceState.Free = false; // Mark the resource as not available
                }
            }).Run();
            
            freeResources.Dispose();
            assignedResources.Dispose();
        }
    }
}