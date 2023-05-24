using Unity.Entities;

public struct BeeSettingsSingletonComponent : IComponentData
{
    public float flightJitter;
    public float damping;
    public float aggressionPercentage;

    public float chaseForce;
    public float carryForce;
    public float attackForce;

    public float grabDistance;
    public float attackDistance;
    public float hitDistance;
}
