using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Intersection {
	public int id;
	public Vector3 position;
	public Vector3Int index;
	public float3 normal;

	public List<Intersection> neighbors;
	public List<TrackSpline> neighborSplines;

	public bool[] occupied;

	public Intersection(Vector3Int intPos, Vector3 pos, float3 norm) {
		index = intPos;
		position = pos;
		normal = norm;
		neighbors = new List<Intersection>();
		neighborSplines = new List<TrackSpline>();

		// each side (top/bottom) has its own "occupied" flag
		// (a car on the underside doesn't block a car on the top)
		occupied = new bool[2];
	}

	public Matrix4x4 GetMatrix()
	{
		var scale = new Vector3(
			RoadGeneratorDots.intersectionSize,
			RoadGeneratorDots.intersectionSize,
			RoadGeneratorDots.trackThickness); 
		return Matrix4x4.TRS(position, Quaternion.LookRotation(normal), scale);
	}
}
