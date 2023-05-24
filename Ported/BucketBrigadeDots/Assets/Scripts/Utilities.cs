using Unity.Mathematics;
using UnityEngine;

public static class Utilities
{
    public static Color ToColor(this float4 data) => new Color(data.x, data.y, data.z, data.w);
    public static float4 ToFloat4(this Color data) => new float4(data.r, data.g, data.b, data.a);
}