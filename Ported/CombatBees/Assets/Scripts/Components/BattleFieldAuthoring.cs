using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BattleField : IComponentData
{
	public float3 Bounds;
	public float HiveDistance;
	public Entity BeeSpawner;

	public Entity BloodSpawner;

	public Entity SmokeSpawner;
}