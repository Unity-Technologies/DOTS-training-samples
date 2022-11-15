using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Field {
	public static Vector3 size = new Vector3(100f,20f,30f);
	public static float gravity = -20f;

	/// <summary>
	/// Get bounds assuming that the field container is at (0,0,0) with central pivot
	/// </summary>
	public static float3 BoundsMin = new float3(-size.x/2, -size.y / 2, -size.z/2);
	public static float3 BoundsMax = new float3(size.x/2, size.y / 2, size.z/2);

	/// <summary>
	/// Get bounds for initial resource spawning assuming that the field container is at (0,0,0) with central pivot
	/// </summary>
	/// <remarks>Consider having a baked entity for config instead</remarks>
	public static float3 ResourceBoundsMin = new float3(-size.x/3, -size.y / 2, -size.z/2);
	public static float3 ResourceBoundsMax = new float3(size.x/3, size.y / 2, size.z/2);
	
	public static float GroundLevel = BoundsMin.y;
}
