using Unity.Entities;
using Unity.Mathematics;

public struct TrainId : IComponentData
{
    public uint value;
}

public struct PlatformId : IComponentData
{
    public uint value;
}

public struct CurrentPathIndex : IComponentData
{
    public int pathLookupIdx;
    public int connectionIdx;
}

public struct Direction : IComponentData
{
    public float3 value;
}

public struct Speed : IComponentData
{
    public float value;
}

public struct TargetPosition : IComponentData
{
    public float3 value;
}

public struct DistanceToTarget : IComponentData
{
    public float value;
}

public struct TrainLine : IComponentData
{
    public BlobAssetReference<Curve> line;
}

public struct BezierTOffset : IComponentData
{
    public float offset;
}

public struct BezierPt
{
    const float k_BezierHandleReach = 0.1f;

    public int index;
    public float3 location, handle_in, handle_out;
    public float distanceAlongPath;

    public BezierPt(int idx, float3 _location, float3 _handle_in, float3 _handle_out)
    {
        index = idx;
        location = _location;
        handle_in = _handle_in;
        handle_out = _handle_out;
        distanceAlongPath = 0;
    }
}

public struct Curve
{
    public BlobArray<BezierPt> points;
}

public struct PlatformTag : IComponentData { }

// TRAIN STATES
public struct EN_ROUTE : IComponentData { }
public struct ARRIVING : IComponentData { }
public struct DOORS_OPEN : IComponentData { }
public struct UNLOADING : IComponentData { }
public struct LOADING : IComponentData { }
public struct DOORS_CLOSE : IComponentData { }
public struct DEPARTING : IComponentData { }
public struct EMERGENCY_STOP : IComponentData { }

// COMMUTER STATES
public struct WALK : IComponentData { }
public struct QUEUE : IComponentData { }
public struct GET_ON_TRAIN : IComponentData { }
public struct WAIT_FOR_STOP : IComponentData { }
public struct GET_OFF_TRAIN : IComponentData { }
