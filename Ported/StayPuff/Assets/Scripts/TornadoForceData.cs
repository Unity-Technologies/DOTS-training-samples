using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TornadoForceData : IComponentData
{
	public float tornadoForce;
	public float tornadoMaxForceDist;
	public float tornadoHeight;
	public float tornadoUpForce;
	public float tornadoInwardForce;
	public float tornadoFader;
	public Vector2 tornadoForceRand;
}