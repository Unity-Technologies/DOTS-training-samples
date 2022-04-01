using Unity.Entities;

[GenerateAuthoringComponent]
public struct HeldResourceComponent : IComponentData
{
	public Entity Value;
}
