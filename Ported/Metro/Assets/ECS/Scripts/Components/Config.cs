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
    public float BezierHandleReach;
    public float BezierPlatformOffset;
    public float PlatformAdjacencyLimit;
    public int BezierMeasurementSubdivisions;
    public float PlatformArrivalThreshold;
    public float RailSpacing;
}