using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Entities;
using Vector2 = UnityEngine.Vector2;

public struct Obstacle : IComponentData
{
	public Vector2 Position;
	public float Radius;
}