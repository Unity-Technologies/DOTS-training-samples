using Unity.Mathematics;

public static class Utils
{
    public static float3 AASignedDistanceToTarget(this AABB box, float3 position, float3 target)
    {
        return target - (position+box.center) - box.halfSize;
    }
}
