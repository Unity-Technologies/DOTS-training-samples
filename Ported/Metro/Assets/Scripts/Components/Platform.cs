using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct Platform : IComponentData
{    
    public NativeArray<float3x2> PlatformQueues; // c0 = worldPosition , c1 = forward (direction)
    public float3 WalkwayFrontLower;
    public float3 WalkwayFrontUpper;
    public float3 WalkwayBackLower;
    public float3 WalkwayBackUpper;
}

public struct TrainOnPlatform: IComponentData
{
    public Entity Train;
}