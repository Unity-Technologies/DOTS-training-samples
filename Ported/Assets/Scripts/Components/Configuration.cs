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
    public float TrailAddSpeed;
    public float TrailDecay;
    public int RotationResolution;
    public int ObstacleRingCount;
    public int ObstaclesPerRing;
    public float ObstacleRadiusStepPerRing;
    public float ObstacleRadius;
    
    public float AntMaxSpeed;
    public float AntMaxTurn;
    public float AntAcceleration;
    public float AntWanderRadius;
    public float AntWanderDistance;
    public float AntObstacleAvoidanceDistance;
    
    public float WanderingStrength;
    public float PheromoneStrength;
    public float ContainmentStrength;
    public float GeneralDirectionStrength;
    public float ProximityStrength;
    public float AvoidanceStrength;
}
