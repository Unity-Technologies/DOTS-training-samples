using Unity.Entities;

[GenerateAuthoringComponent]
public struct SteeringSettings : IComponentData
{
    public float RandomDeviationDegrees;
    public float SteeringSmoothingFrequency;
    public float SteeringDampingRatio;

    public float GoalSteeringStrength;
    public float PheromoneSteeringStrength;

    public float GoalSteeringDistanceStart;     // At this distance we start to get a steering bonus
    public float GoalSteeringDistanceEnd;       // At this distnace we get max steering bonus
}
