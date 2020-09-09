using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BattleField : IComponentData
{
	public float3 Bounds;
	public float HiveDistance;
}