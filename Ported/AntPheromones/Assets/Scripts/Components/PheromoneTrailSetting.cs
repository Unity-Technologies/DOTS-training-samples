using Unity.Entities;

public struct PheromoneTrailSetting : IComponentData
{
    public float Decay;
    public float Speed;
}
