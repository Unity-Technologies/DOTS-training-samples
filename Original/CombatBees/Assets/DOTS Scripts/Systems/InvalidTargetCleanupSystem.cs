using Unity.Entities;

public class InvalidTargetCleanupSystem : SystemBase
{
    private ComponentTypes typesToRemove;
    private EntityQuery beesNeedingCleanup;
    protected override void OnCreate()
    {
        ComponentType[] types = new ComponentType[]
        {
            ComponentType.ReadOnly<MoveTarget>(),
            ComponentType.ReadOnly<TargetPosition>(),
            ComponentType.ReadOnly<CarriedFood>(), 
            ComponentType.ReadOnly<FetchingFoodTag>(),
            ComponentType.ReadOnly<AttackingBeeTag>(),
            ComponentType.ReadOnly<BeeWithInvalidtarget>(),
        };

        typesToRemove = new ComponentTypes(types);
        beesNeedingCleanup = GetEntityQuery(typeof(BeeWithInvalidtarget));
    }

    protected override void OnUpdate()
    {
        EntityManager.RemoveComponent(beesNeedingCleanup, typesToRemove);
    }
}