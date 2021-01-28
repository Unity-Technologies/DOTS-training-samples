using Unity.Entities;

public class InvalidTargetCleanupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var beesNeedingCleanup = GetEntityQuery(typeof(BeeWithInvalidtarget));
        EntityManager.RemoveComponent<MoveTarget>(beesNeedingCleanup);
        EntityManager.RemoveComponent<TargetPosition>(beesNeedingCleanup);
        EntityManager.RemoveComponent<CarriedFood>(beesNeedingCleanup);
        EntityManager.RemoveComponent<FetchingFoodTag>(beesNeedingCleanup);
        EntityManager.RemoveComponent<AttackingBeeTag>(beesNeedingCleanup);
        EntityManager.RemoveComponent<BeeWithInvalidtarget>(beesNeedingCleanup);
    }
}