using Unity.Entities;

[GenerateAuthoringComponent]
public struct TornadoForceData : IComponentData
{
	public float tornadoInwardForce;
}