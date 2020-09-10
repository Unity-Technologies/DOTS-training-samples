using Unity.Entities;

[GenerateAuthoringComponent]
public struct Speed : IComponentData
{
	public float TopSpeed;
	public float Acceleration;
}