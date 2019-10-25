using Unity.Entities;
using Unity.Mathematics;

public struct TrainId : IComponentData
{
    public int value;
}

public struct CarriageId : IComponentData
{
    public int value;
}

public struct PlatformId : IComponentData
{
    public int value;
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

public struct Timer : IComponentData
{
    public float value;
}public struct TimeInterval : IComponentData
{
    public float value;
}

public struct BezierCurve : IComponentData
{
    public BlobAssetReference<Curve> line;
}

public struct BezierTOffset : IComponentData
{
    public float offset;
    public float renderOffset;
}

public struct StationData : IBufferElementData
{
    public float start;
    public float end;
    public int platformId;
}

public struct BezierPt
{
    const float k_BezierHandleReach = 0.1f;

    public int index;
    public float3 location, handle_in, handle_out;
    public float distanceAlongPath;

    public BezierPt(BezierPoint pt)
    {
        index = pt.index;
        location = pt.location;
        handle_in = pt.handle_in;
        handle_out = pt.handle_out;
        distanceAlongPath = pt.distanceAlongPath;
    }
}

public struct Curve
{
    public BlobArray<BezierPt> points;
    public float distance;
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
