using Unity.Mathematics;
using UnityEngine;

public class Point
{
	public float3 pos;
	public float3 old;

	public bool anchor;

	public int neighborCount;

	public void CopyFrom(Point other)
	{
		pos = other.pos;
		old = other.old;
		anchor = other.anchor;
		neighborCount = other.neighborCount;
	}
}
