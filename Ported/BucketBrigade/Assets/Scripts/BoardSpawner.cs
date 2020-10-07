using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct BoardSpawner : IComponentData
{
	public Entity Prefab;

	[Min(0)]
	public int SizeX;
	[Min(0)]
	public int SizeZ;

	public float RandomYOffset;

	[Range(-100.0f, 100.0f)]
	public float InitialIntensity;

	[Min(0)]
	public int InitialOnFireCellCount;
	[Range(-100.0f, 100.0f)]
	public float InitialOnFireIntensityMin;
	[Range(-100.0f, 100.0f)]
	public float InitialOnFireIntensityMax;
}
