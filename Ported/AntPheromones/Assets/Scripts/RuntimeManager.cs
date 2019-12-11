using Unity.Collections;
using Unity.Mathematics;

public class RuntimeManager
{
    protected static RuntimeManager i = null;
	public NativeArray<int2> obstacleBuckets;
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