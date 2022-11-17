using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Field {
	public static readonly Vector3 size = new Vector3(300f,50f,100f);
	public static readonly float gravity = -20f;

	/// <summary>
	/// Get bounds assuming that the field container is at (0,0,0) with central pivot
	/// </summary>
	public static readonly float3 BoundsMin = new float3(-size.x/2, -size.y / 2, -size.z/2);
	public static readonly float3 BoundsMax = new float3(size.x/2, size.y / 2, size.z/2);

	/// <summary>
	/// Get bounds for initial resource spawning assuming that the field container is at (0,0,0) with central pivot
	/// </summary>
	/// <remarks>Consider having a baked entity for config instead</remarks>
	public static readonly float3 ResourceBoundsMin = new float3(-size.x/6, -size.y / 2, -size.z/2);
	public static readonly float3 ResourceBoundsMax = new float3(size.x/6, size.y / 2, size.z/2);
	
	public static readonly float GroundLevel = BoundsMin.y;
	public static readonly int GridSize = (int) size.x * (int) size.z;
	
	public static bool InTeamArea(float xPos, float3 hivePos)
	{
		return (xPos / hivePos.x) >= 1.0f;
	}
}
