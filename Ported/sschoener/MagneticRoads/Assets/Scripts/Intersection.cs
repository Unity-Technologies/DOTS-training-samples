using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Intersections
{
	public static int Count;
	public static float3[] Position;
	public static int3[] Index;
	public static float3[] Normal;

	public static List<int>[] Neighbors;
	public static List<int>[] NeighborSplines;
	
	// each side (top/bottom) has its own "occupied" flag
	// (a car on the underside doesn't block a car on the top)
	public static bool[][] Occupied;

	public static void Init(int n)
	{
		Count = n;
		Position = new float3[n];
		Index = new int3[n];
		Normal = new float3[n];
		Neighbors = new List<int>[n];
		NeighborSplines = new List<int>[n];
		Occupied = new bool[n][];
		for (int i = 0; i < n; i++)
		{
			Neighbors[i] = new List<int>();
			NeighborSplines[i] = new List<int>();
			Occupied[i] = new bool[2];
		}
	}

	public static Matrix4x4 GetMatrix(int i)
	{
		var scale = new Vector3(
			RoadGeneratorDots.intersectionSize,
			RoadGeneratorDots.intersectionSize,
			RoadGeneratorDots.trackThickness); 
		return Matrix4x4.TRS(Position[i], Quaternion.LookRotation(Normal[i]), scale);
	}
}