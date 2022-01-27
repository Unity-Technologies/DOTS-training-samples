using Unity.Collections;
using Unity.Entities;

public partial class BeeResourceTargeting : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }

    protected override void OnUpdate()
    {
        NativeList<Entity> freeResources = new NativeList<Entity>(Allocator.TempJob);
        NativeList<Entity> assignedResources = new NativeList<Entity>(Allocator.TempJob);
        
        Entities.WithAll<ResourceTag>().ForEach((Entity entity, in Targeted targeted) =>
        {
            if (!targeted.Value) freeResources.Add(entity); // Find free resources (not targeted or home)
        }).Run();
        
        if (freeResources.Length > 0)
        {
            Entities.WithAll<BeeTag>().ForEach((ref BeeTargets beeTargets, ref RandomState randomState) =>
            {
                if (beeTargets.ResourceTarget == Entity.Null) // if bee does not have a target
                {
                    // Assign a random resource
                    int randomResourceIndex = randomState.Random.NextInt(freeResources.Length);
                    beeTargets.ResourceTarget = freeResources.ElementAt(randomResourceIndex);
                    freeResources.RemoveAt(randomResourceIndex); // Remove from the list of available resources
                    assignedResources.Add(beeTargets.ResourceTarget); // Add to the list used in the next step
                }
            }).Run();
        }
        
        Entities.WithAll<ResourceTag>().ForEach((Entity entity, ref Targeted targeted) =>
        {
            bool assigned = false;
            
            foreach (var assignedResource in assignedResources)
            {
                if (entity == assignedResource) assigned = true; // The resource been assigned to a bee
            }
            
            if (assigned)
            {
                targeted.Value = true; // Mark the resource as not available
            }
        }).Run();
        
        freeResources.Dispose();
        assignedResources.Dispose();
    }
}