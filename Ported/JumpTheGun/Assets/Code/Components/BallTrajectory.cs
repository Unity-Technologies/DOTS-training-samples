using Unity.Entities;
using Unity.Mathematics;

public struct BallTrajectory : IComponentData
{
    public float3 Source;
    public float3 Destination;
}