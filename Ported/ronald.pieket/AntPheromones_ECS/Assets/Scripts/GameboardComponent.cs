using Unity.Entities;
using Unity.Mathematics;

public struct ObstacleList
{
  public BlobArray<float2> Positions;
}

public struct ObstacleGrid
{
  public BlobArray<ObstacleList> Buckets;
}

public struct GameboardComponent : IComponentData
{
  public BlobAssetReference<ObstacleGrid> ObstacleGrid;
  public BlobAssetReference<ObstacleList> ObstacleList;
  public float2 ColonyPosition;
  public float2 ResourcePosition;
}
