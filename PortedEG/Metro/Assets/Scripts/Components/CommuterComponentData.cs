using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

// An empty component is called a "tag component".
struct CommuterTag : IComponentData
{
}

struct CommuterSpeed : IComponentData
{
    public float3 Value;
    public float acceleration;
}

struct CommuterPlatformIDInfo : IComponentData
{
    public int CurrentPlatformID;
    public int NextPlatformID;

    public int StartPlatformID;
    public int FinalPlatformID;
}

public struct CommuterTaskUnmanaged
{
    public int State;
    // Using a native array will make edit crash!
    public float4x4 Destinations;
    public int DestinationIndex;
    public int DestinationCount;
    public int StartPlatform;
    public int EndPlatform;
    public WalkwayConnectInfo WalkwayInfo;

    public int DestNum;
}

public struct CommuterStateInfo : IComponentData
{
    public NativeQueue<CommuterTaskUnmanaged> TaskList;
    public int CurrentTaskIndex;
    public int NeedNextTask;

    public CommuterTaskUnmanaged CurrentTask;

    public Entity WalkwaysEntity;
}