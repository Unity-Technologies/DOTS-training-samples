using Unity.Entities;

public struct ConfigComponent : IComponentData
{
  public int MapSize;
  public int BucketResolution;
  public int AntCount;
  public float RandomSteerStrength;
  public float PheromoneSteerStrength;
  public float WallSteerStrength;
  public float GoalSteerStrength;
  public float OutwardStrength;
  public float InwardStrength;
  public float DecayRate;
  public float DropRate;
  public float ObstacleRadius;
  public float ObstacleRingCount;
  public float ObstaclesPerRing;
  public float TargetRadius;
}
