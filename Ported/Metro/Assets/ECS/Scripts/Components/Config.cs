using Unity.Entities;

public struct Config : IComponentData
{
    // Prefabs
    public Entity CarriagePrefab;
    public Entity PlatformPrefab;
    public Entity CommuterPrefab;
    public Entity RailPrefab;

    // Bezier data
    public float BezierHandleReach;
    public float BezierPlatformOffset;
    public float PlatformAdjacencyLimit;
    public int BezierMeasurementSubdivisions;
    public float PlatformArrivalThreshold;
    public float RailSpacing;
    
    // Carriage data
    public float CarriageSizePlusPadding;
}