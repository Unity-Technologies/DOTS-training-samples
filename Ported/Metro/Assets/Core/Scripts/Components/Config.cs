using Unity.Entities;

struct Config : IComponentData
{
    public Entity PersonPrefab;
    public Entity PlatformPrefab;
    public Entity RailsPrefab;
    public int PersonCount;
    public int PlatformCountPerStation;
    public int NumberOfStations;
    public float SafeZoneRadius;
}