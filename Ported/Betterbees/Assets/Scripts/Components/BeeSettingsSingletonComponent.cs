using Unity.Entities;

public struct BeeSettingsSingletonComponent : IComponentData
{
    public float flightJitter;
    public float damping;
    public float chaseForce;
    public float carryForce;
    public float interactionDistance;
    public float agressionPercentage;
    public float attackForce;
    public float attackDistance;
}
