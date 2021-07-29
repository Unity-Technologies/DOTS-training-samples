using Unity.Entities;
using Unity.Mathematics;

public struct GeneralSettings : IComponentData
{
    public float NormalExcitement;
    public float HoldingResourceExcitement;
    public float AntSpeed;
    public float RandomSteering;
    public float GoalSteerStrength;
    public float PheromoneSteeringDistance;
    public float PheromoneSteerStrength;
    public float InwardStrength;
    public float OutwardStrength;
}