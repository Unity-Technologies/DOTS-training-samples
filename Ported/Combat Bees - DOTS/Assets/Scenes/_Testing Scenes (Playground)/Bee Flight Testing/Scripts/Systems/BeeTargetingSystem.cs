using Unity.Collections;
using Unity.Entities;

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
            NativeList<Entity> freeResources = new NativeList<Entity>(Allocator.TempJob);
            NativeList<Entity> assignedResources = new NativeList<Entity>(Allocator.TempJob);
        
            Entities.WithAll<Resource>().ForEach((Entity entity, in ResourceState resourceState) =>
            {
                if (resourceState.Free) freeResources.Add(entity); // Find free resources (not targeted or home)
            }).Run();

            if (freeResources.Length > 0)
            {
                Entities.WithAll<Bee>().ForEach((ref BeeTargets beeTargets) =>
                {
                    if (beeTargets.ResourceTarget == Entity.Null) // if bee does not have a target
                    {
                        // Assign a random resource
                        int randomResourceIndex = beeTargets.random.NextInt(freeResources.Length);
                        beeTargets.ResourceTarget = freeResources.ElementAt(randomResourceIndex);
                        freeResources.RemoveAt(randomResourceIndex); // Remove from the list of available resources
                        assignedResources.Add(beeTargets.ResourceTarget); // Add to the list used in the next step
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