using Unity.Entities;

public struct RailSpawnerComponent : IComponentData
{
    public Entity TrackPrefab;
    public Entity PlatformPrefab;
    public Entity TrainPrefab;
    public Entity CarriagePrefab;
    public float RailSpacing;
}