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