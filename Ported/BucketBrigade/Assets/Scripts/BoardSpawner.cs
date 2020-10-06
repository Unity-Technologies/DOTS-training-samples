using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BoardSpawner : IComponentData
{
	public Entity Prefab;
	
	[Min(0)]
	public int SizeX;
	[Min(0)]
	public int SizeZ;
	
	public float RandomYOffset;

	[Range(-100,100)]
	public float InitialIntensity;

	[Min(0)]
	public int InitialOnFireCellCount;
}
