using Unity.Entities;

[GenerateAuthoringComponent]
public struct PheromoneDecay : IComponentData
{
    public float decaySpeed;
}
