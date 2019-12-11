using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Obstacle : IComponentData
{
	public float2 position;
	public float radius;
	public int bucketIndex;
}

struct ObstacleBucket : IComponentData
{
	public int2 range;
}