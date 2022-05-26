using Unity.Entities;

public struct FarmerCombat : IComponentData
{
    public Entity combatTarget;
    public float cooldownTicker;
}
