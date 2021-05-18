using Unity.Mathematics;

public static class Utils
{
    public static Random Random { get; }

    static Utils()
    {
        //Random = new Random((uint)System.DateTime.Now.Ticks);
        Random = new Random(1234);
    }

    public static float3 BoundedRandomPosition(AABB bounds)
    {
        var x = (float)Random.NextDouble(bounds.Min.x, bounds.Max.x);
        var y = (float)Random.NextDouble(bounds.Min.y, bounds.Max.y);
        var z = (float)Random.NextDouble(bounds.Min.z, bounds.Max.z);

        return new float3(x, y, z);
    }

}
