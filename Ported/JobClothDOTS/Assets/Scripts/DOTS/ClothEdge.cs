using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct ClothEdge : IComponentData
{
	public int2 IndexPair;
	public float Length;
}
