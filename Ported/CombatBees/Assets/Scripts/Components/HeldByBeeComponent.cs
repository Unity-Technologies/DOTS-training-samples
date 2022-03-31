using Unity.Entities;

[GenerateAuthoringComponent]
public struct HeldByBeeComponent : IComponentData
{
	public Entity Value;
}
