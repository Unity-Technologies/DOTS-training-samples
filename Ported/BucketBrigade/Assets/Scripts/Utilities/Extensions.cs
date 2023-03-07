using Unity.Mathematics;

public static class Extensions 
{
    public static float4 ToFloat4(this UnityEngine.Color color)
    {
        return new float4(color.r, color.g, color.b, color.a);
    }
}
