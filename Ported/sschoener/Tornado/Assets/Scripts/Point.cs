using Unity.Mathematics;
using UnityEngine;

public struct Point
{
	public float3 pos;
	public float3 old;

	public bool anchor;

	public int neighborCount;
}
