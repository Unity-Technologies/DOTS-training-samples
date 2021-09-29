using Unity.Entities;

[GenerateAuthoringComponent]
public struct PheromoneMap : IBufferElementData
{
	public float intensity;
}