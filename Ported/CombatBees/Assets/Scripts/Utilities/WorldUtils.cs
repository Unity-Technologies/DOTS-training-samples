using Unity.Mathematics;

public struct WorldUtils
{
    public static bool IsInRedHive(in WorldBounds bounds, float3 position)
    {
        return position.x >= (bounds.AABB.Max.x - bounds.HiveOffset);
    }
    
    public static bool IsInBlueHive(in WorldBounds bounds, float3 position)
    {
        return position.x <= (bounds.AABB.Min.x + bounds.HiveOffset);
    }

    public static float3 GetBlueHiveRandomPosition(in WorldBounds bounds, ref Random random)
    {
        return new float3(
            bounds.AABB.Min.x + bounds.HiveOffset * 0.5f,
            random.NextFloat(bounds.AABB.Min.y + 2.0f, bounds.AABB.Max.y),
            random.NextFloat(bounds.AABB.Min.z, bounds.AABB.Max.z));
    }
    
    public static float3 GetRedHiveRandomPosition(in WorldBounds bounds, ref Random random)
    {
        return new float3(
            bounds.AABB.Max.x - bounds.HiveOffset * 0.5f,
            random.NextFloat(bounds.AABB.Min.y + 2.0f, bounds.AABB.Max.y),
            random.NextFloat(bounds.AABB.Min.z, bounds.AABB.Max.z));
    }
    
    public static float3 GetRandomInBoundsPosition(in WorldBounds bounds, ref Random random)
    {
        return new float3(
            random.NextFloat(bounds.AABB.Min.x + bounds.HiveOffset, bounds.AABB.Max.x - bounds.HiveOffset),
            random.NextFloat(bounds.AABB.Min.y + 2.0f, bounds.AABB.Max.y),
            random.NextFloat(bounds.AABB.Min.z, bounds.AABB.Max.z));
    }
}