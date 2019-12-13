
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct AntComponent : IComponentData
{
	public float facingAngle;
	public float speed;
	public float brightness;
	public int index;
	public int state;
	public bool stateSwitch;
	public float2 velocity;
}
