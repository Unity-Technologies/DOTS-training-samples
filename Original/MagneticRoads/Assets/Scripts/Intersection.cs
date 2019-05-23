using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection {
	public int id;
	public Vector3 position;
	public Vector3Int index;
	public Vector3Int normal;

	public List<Intersection> neighbors;
	public List<TrackSpline> neighborSplines;

	public bool[] occupied;

	public Intersection(Vector3Int intPos, Vector3 pos, Vector3Int norm) {
		index = intPos;
		position = pos;
		normal = norm;
		neighbors = new List<Intersection>();
		neighborSplines = new List<TrackSpline>();

		// each side (top/bottom) has its own "occupied" flag
		// (a car on the underside doesn't block a car on the top)
		occupied = new bool[2];
	}

	public Matrix4x4 GetMatrix() {
		return Matrix4x4.TRS(position,Quaternion.LookRotation(normal),new Vector3(RoadGenerator.intersectionSize,RoadGenerator.intersectionSize,RoadGenerator.trackThickness));
	}
}
