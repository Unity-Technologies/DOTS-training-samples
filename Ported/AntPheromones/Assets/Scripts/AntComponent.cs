
using Unity.Entities;

[GenerateAuthoringComponent]
public struct AntComponent : IComponentData
{
	public float acceleration;
	public float facingAngle;
	public float speed;
}
