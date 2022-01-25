using Unity.Entities;
using Unity.Mathematics;

public struct Configuration : IComponentData
{
    public float4 SearchColor;
    public float4 CarryColor;
    public int AntCount;
    public int MapSize;
    public int BucketResolution;
    public float3 AntSize;
    public float AntSpeed;
    public float AntAccel;
    public float TrailAddSpeed;
    public float TrailDecay;
    public float RandomSteering;
    public float PheromoneSteerStrength;
    public float WallSteerStrength;
    public float GoalSteerStrength;
    public float OutwardStrength;
    public float InwardStrength;
    public int RotationResolution;
    public int ObstacleRingCount;
    public float ObstaclesPerRing;
    public float ObstacleRadius;
}
