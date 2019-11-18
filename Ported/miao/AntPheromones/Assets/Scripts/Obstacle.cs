using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;
using Vector2 = UnityEngine.Vector2;

public struct Obstacle : IComponentData
{
	public float3 Position;
	public float Radius;
}