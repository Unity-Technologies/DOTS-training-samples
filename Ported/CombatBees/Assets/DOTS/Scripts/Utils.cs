using Unity.Mathematics;

public static class Utils
{
    public static float3 BoundedRandomPosition(AABB bounds, Random random)
    {
        var x = random.NextFloat(bounds.Min.x, bounds.Max.x);
        var y = random.NextFloat(bounds.Min.y, bounds.Max.y);
        var z = random.NextFloat(bounds.Min.z, bounds.Max.z);

        return new float3(x, y, z);
    }

}
