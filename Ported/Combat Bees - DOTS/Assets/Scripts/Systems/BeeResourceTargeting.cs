using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial class BeeResourceTargeting : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }

    protected override void OnUpdate()
    {
        NativeList<Entity> freeResources = GetFreeResources();
        NativeList<Entity> assignedResources = AssignResourcesToBees(freeResources);
        
        MarkTargetedResources(assignedResources);
        
        var allTranslations = GetComponentDataFromEntity<Translation>(true);
                
        Entities.WithAll<BeeTag>().WithNativeDisableContainerSafetyRestriction(allTranslations).ForEach((ref BeeTargets beeTargets, in HeldItem heldItem, in Translation translation, in BeeStatus beeStatus,in BeeDead beeDead) =>
        {
            if(beeStatus.Value == Status.Gathering && !beeDead.Value){
                if (heldItem.Value != Entity.Null)
                {
                    // Switch target to home if holding a resource
                    beeTargets.CurrentTargetPosition = beeTargets.HomePosition;
                    beeTargets.CurrentTargetPosition.z = translation.Value.z;
                }
                else if (beeTargets.ResourceTarget != Entity.Null)
                {
                    // If has a target resource & not holding it, then go for it
                    beeTargets.CurrentTargetPosition = allTranslations[beeTargets.ResourceTarget].Value;
                }
            }
        }).Run();
        
        freeResources.Dispose();
        assignedResources.Dispose();
    }

    private NativeList<Entity> GetFreeResources()
    {
        NativeList<Entity> freeResources = new NativeList<Entity>(Allocator.TempJob);
        
        Entities.WithAll<ResourceTag>().ForEach((Entity entity, in Targeted targeted) =>
        {
            if (!targeted.Value) freeResources.Add(entity); // Find free resources (not targeted or home)
        }).Run();

        return freeResources;
    }

    private NativeList<Entity> AssignResourcesToBees(NativeList<Entity> freeResources)
    {
        NativeList<Entity> assignedResources = new NativeList<Entity>(Allocator.TempJob);

        if (freeResources.Length > 0)
        {
            Entities.WithAll<BeeTag>().ForEach((ref BeeTargets beeTargets, ref RandomState randomState, in BeeStatus beeStatus, in BeeDead beeDead) =>
            {
                if (beeTargets.ResourceTarget == Entity.Null && beeStatus.Value == Status.Gathering && !beeDead.Value) // if bee does not have a target
                {
                    // Assign a random resource
                    int randomResourceIndex = randomState.Value.NextInt(freeResources.Length);
                    beeTargets.ResourceTarget = freeResources.ElementAt(randomResourceIndex);
                    freeResources.RemoveAt(randomResourceIndex); // Remove from the list of available resources
                    assignedResources.Add(beeTargets.ResourceTarget); // Add to the list used in the next step
                }
            }).Run();
        }

        return assignedResources;
    }

    private void MarkTargetedResources(NativeList<Entity> assignedResources)
    {
        if (assignedResources.Length > 0)
        {
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
        }
    }
}