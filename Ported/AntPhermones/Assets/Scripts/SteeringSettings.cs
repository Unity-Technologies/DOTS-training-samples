using Unity.Entities;

[GenerateAuthoringComponent]
public struct SteeringSettings : IComponentData
{
    public float RandomDeviationDegrees;
    public float SteeringSmoothingFrequency;
    public float SteeringDampingRatio;

    public float GoalSteeringStrength;
    public float PheromoneSteeringStrength;
}
