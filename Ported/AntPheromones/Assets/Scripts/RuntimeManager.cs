using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class RuntimeManager
{
    private static RuntimeManager i = null;

    public int2 obstacleBucketDimensions;
	public NativeArray<int2> obstacleBuckets;
    public NativeArray<MapObstacle> cachedObstacles;
    public float3 colonyPosition;
    public float3 resourcePosition;
    
    public static RuntimeManager instance
    {
        get 
        {
            if (i == null)
                i = new RuntimeManager();

            return i;
        }
    }
}