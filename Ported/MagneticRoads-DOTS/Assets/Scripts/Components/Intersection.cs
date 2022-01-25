using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct IntersectionComponent : ISharedComponentData, IEquatable<IntersectionComponent>
{
	public List<TrackSpline> neighborSplines;
	public Vector3Int normal;

	public bool Equals(IntersectionComponent other)
	{
		return Equals(neighborSplines, other.neighborSplines);
	}

	public override bool Equals(object obj)
	{
		return obj is IntersectionComponent other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (neighborSplines != null ? neighborSplines.GetHashCode() : 0);
	}
}
public class Intersection
{
	public int id;
	public Vector3 position;
	public Vector3Int index;
	public Vector3Int normal;

	public List<TrackSpline> neighborSplines;

	public bool[] occupied;

	public Intersection(Vector3Int intPos, Vector3 pos, Vector3Int norm)
	{
		id = 0;
		index = intPos;
		position = pos;
		normal = norm;
		neighborSplines = new List<TrackSpline>();

		// each side (top/bottom) has its own "occupied" flag
		// (a car on the underside doesn't block a car on the top)
		occupied = new bool[2];
	}

	public Matrix4x4 GetMatrix() {
		return Matrix4x4.TRS(position,Quaternion.LookRotation(normal),new Vector3(RoadGenerator.intersectionSize,RoadGenerator.intersectionSize,RoadGenerator.trackThickness));
	}
}
