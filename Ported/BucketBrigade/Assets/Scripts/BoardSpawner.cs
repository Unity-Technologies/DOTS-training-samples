using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BoardSpawner : IComponentData
{
	public Entity Prefab;
	public int SizeX;
	public int SizeZ;
	public float RandomYOffset;
}
