using Unity.Mathematics;
using UnityEngine;

public static class Extensions
{
    public static int3 ToInt3(this Vector3Int v3)
    {
        return new int3(v3.x, v3.y, v3.z);
    }
    
    public static Vector3 ToVector3(this int3 v3)
    {
        return new Vector3(v3.x, v3.y, v3.z);
    }
    
    public static Vector3 ToVector3(this float3 v3)
    {
        return new Vector3(v3.x, v3.y, v3.z);
    }
}
