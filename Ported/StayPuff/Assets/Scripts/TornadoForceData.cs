using Unity.Entities;

[GenerateAuthoringComponent]
public struct TornadoForceData : IComponentData
{
	public float tornadoForce;
	public float tornadoMaxForceDist;
	public float tornadoHeight;
	public float tornadoUpForce;
	public float tornadoInwardForce;
	public float tornadoFader;
}