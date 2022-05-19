using Unity.Entities;

public struct Config : IComponentData
{
    // Prefabs
    public Entity CarriagePrefab;
    public Entity PlatformPrefab;
    public Entity CommuterPrefab;
    public Entity RailPrefab;
    
    // Trail data
    public int TrainsToSpawn;
    
    // Bezier data
    public float BEZIER_HANDLE_REACH;
    public float BEZIER_PLATFORM_OFFSET;
    public float PLATFORM_ADJACENCY_LIMIT;
    public int BEZIER_MEASUREMENT_SUBDIVISIONS;
    public float PLATFORM_ARRIVAL_THRESHOLD;
    public float RAIL_SPACING;
}