using Unity.Entities;

[GenerateAuthoringComponent]
public struct Tuning : IComponentData
{
	public float Speed;
	public float AntAngleRange;
	public int PheromoneBuffer;
}
