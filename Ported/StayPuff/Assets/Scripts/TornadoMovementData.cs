using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TornadoMovementData : IComponentData
{
	public float loopsize;
	public float looprate;
	public float loopseed;
	public Vector3 loopposition;
}

