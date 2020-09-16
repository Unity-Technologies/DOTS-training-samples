using Unity.Entities;

[GenerateAuthoringComponent]
public struct SteeringSettings : IComponentData
{
    public float PheromoneDeviationDegrees;
    public float SteeringSmoothingFrequency;
    public float SteeringDampingRatio;
}
