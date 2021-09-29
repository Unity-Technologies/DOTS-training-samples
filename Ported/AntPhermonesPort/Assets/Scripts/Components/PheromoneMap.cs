using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PheromoneMap : IBufferElementData
{
	public float4 intensity;
}