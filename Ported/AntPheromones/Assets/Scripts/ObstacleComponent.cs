
using Unity.Entities;

[GenerateAuthoringComponent]
public struct ObstacleComponent : IComponentData
{
	public float radius;
	public int bucketIndex;
}
