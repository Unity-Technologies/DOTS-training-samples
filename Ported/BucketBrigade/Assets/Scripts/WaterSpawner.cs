using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public enum Side
{
	North,
	East,
	South,
	West
}

[GenerateAuthoringComponent]
public struct WaterSpawner : IComponentData
{
	public Entity Prefab;
	public Side Side;
	public float3 Offset;
	public float RotationY;
}
