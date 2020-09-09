using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ResourceSpawner : IComponentData
{
	public Entity ResourcePrefab;
	public int SizeX;
	public int SizeZ;
}