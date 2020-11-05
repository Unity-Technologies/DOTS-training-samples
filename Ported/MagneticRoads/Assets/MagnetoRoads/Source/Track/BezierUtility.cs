using Unity.Mathematics;

public static class BezierUtility
{
    public static float3 EvaluateBezier(float3 startPoint, float3 anchor1, float3 anchor2, float3 endPoint, float t)
    {
        t = math.clamp(t, 0, 1);
        return startPoint * (1f - t) * (1f - t) * (1f - t) + 3f * anchor1 * (1f - t) * (1f - t) * t +
               3f * anchor2 * (1f - t) * t * t + endPoint * t * t * t;;
    }
}
