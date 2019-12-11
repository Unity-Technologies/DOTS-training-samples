using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class RuntimeManager
{
    public struct CachedObstacle
    {
        public float3 position;
        public float radius;
    }

    private static RuntimeManager i = null;
    
	public NativeArray<int2> obstacleBuckets;
    public NativeArray<CachedObstacle> cachedObstacles;
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

    ~RuntimeManager()
    {
        if (obstacleBuckets != null)
            obstacleBuckets.Dispose();
    }
}