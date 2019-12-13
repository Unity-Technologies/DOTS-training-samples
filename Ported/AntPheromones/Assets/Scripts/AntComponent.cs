
using Unity.Entities;

[GenerateAuthoringComponent]
public struct AntComponent : IComponentData
{
	public float acceleration;
	public float facingAngle;
	public float speed;
	public float brightness;
	public int index;
	public int state;
	public bool stateSwitch;
}
