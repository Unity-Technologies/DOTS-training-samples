using Unity.Entities;
using Unity.Mathematics;


public struct WalkwayConnectInfo
{
    public int platform_Connects_FROM;
    public int platform_Connects_TO;

    public float3 nav_START;
    public float3 nav_END;
}

public struct WalkwayInfo
{
    public int fPlatform_Connects_FROM;
    public int fPlatform_Connects_TO;

    public int bPlatform_Connects_FROM;
    public int bPlatform_Connects_TO;

    public float3 fPos;
    public float3 bPos;

    public float3 fnav_START;
    public float3 fnav_END;

    public float3 bnav_START;
    public float3 bnav_END;
}

struct WalkwayComponent : IComponentData
{
    WalkwayInfo info;
}
