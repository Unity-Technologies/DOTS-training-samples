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
        NativeList<Entity> resources = GetResources();
        AssignResourcesToBees(resources);
        
        var allTranslations = GetComponentDataFromEntity<Translation>(true);
                
        Entities.WithAll<BeeTag>().WithNativeDisableContainerSafetyRestriction(allTranslations).ForEach((ref BeeTargets beeTargets, in HeldItem heldItem, in Translation translation, in BeeStatus beeStatus,in BeeDead beeDead) =>
        {
            if (beeStatus.Value == Status.Gathering && !beeDead.Value)
            {
                if (heldItem.Value != Entity.Null) // item grabbed
                {
                    // Switch target to home if holding a resource
                    beeTargets.CurrentTargetPosition = beeTargets.HomePosition;
                    beeTargets.CurrentTargetPosition.z = translation.Value.z; // keep going straight back
                }
                else if (beeTargets.ResourceTarget != Entity.Null)
                {
                    // If has a target resource & not holding it, then go for it
                    beeTargets.CurrentTargetPosition = allTranslations[beeTargets.ResourceTarget].Value;
                }
            }
        }).Run();
        
        resources.Dispose();
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
    private NativeList<Entity> GetResources()
    {
        NativeList<Entity> resources = new NativeList<Entity>(Allocator.TempJob);
        
        Entities.WithAll<ResourceTag>().ForEach((Entity entity, in Targeted targeted) =>
        {
            if (!targeted.Value)
            {
                resources.Add(entity); // Find free resources (not targeted or home)
            }
        }).Run();

        return resources;
    }

    private void AssignResourcesToBees(NativeList<Entity> freeResources)
    {
            Entities.WithAll<BeeTag>().ForEach((ref BeeTargets beeTargets, ref RandomState randomState, in BeeStatus beeStatus, in BeeDead beeDead) =>
            {
                if (beeTargets.ResourceTarget == Entity.Null && beeStatus.Value == Status.Gathering && !beeDead.Value) // if bee does not have a resource target
                {
                    // Assign a random resource
                    if (freeResources.Length > 0)
                    {
                        int randomResourceIndex = randomState.Value.NextInt(freeResources.Length);
                        beeTargets.ResourceTarget = freeResources.ElementAt(randomResourceIndex);
                        freeResources.RemoveAt(randomResourceIndex); // Remove from the list of available resources
                        
                        var targetedComponent = GetComponent<Targeted>(beeTargets.ResourceTarget);
                        targetedComponent.Value = true;
                        SetComponent(beeTargets.ResourceTarget, targetedComponent);
                    }
                }
            }).Run();
    }
}