using Unity.Entities;

public struct BeeProperties : IComponentData
{
    public float ChaseForce;
    public float Damping;
    public float FlightJitter;
    public float RotationStiffness;
    public float TeamAttraction;
    public float KillingReach;
    public float AttackDashReach;
}
