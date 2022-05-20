using Unity.Entities;
using UnityEngine;

class ConfigAuthoring : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject CarriagePrefab;
    public GameObject PlatformPrefab;
    public GameObject CommuterPefab;
    public GameObject RailPrefab;
    
    [Header("Train data")]
    public int TrainsToSpawn;
    
    [Header("Bezier data")]
    public float BezierHandleReach = 0.15f;
    public float BezierPlatformOffset = 8f;
    public float PlatformAdjacencyLimit = 12f;
    public int BezierMeasurementSubdivisions = 2;
    public float PlatformArrivalThreshold = 0.975f;
    public float RailSpacing = 0.5f;

    public float CarriageSizePlusPadding = 6f;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            CarriagePrefab = GetEntity(authoring.CarriagePrefab),
            PlatformPrefab = GetEntity(authoring.PlatformPrefab),
            CommuterPrefab = GetEntity(authoring.CommuterPefab),
            RailPrefab = GetEntity(authoring.RailPrefab),

            BezierHandleReach = authoring.BezierHandleReach,
            BezierPlatformOffset = authoring.BezierPlatformOffset,
            PlatformAdjacencyLimit = authoring.PlatformAdjacencyLimit,
            BezierMeasurementSubdivisions = authoring.BezierMeasurementSubdivisions,
            PlatformArrivalThreshold = authoring.PlatformArrivalThreshold,
            RailSpacing = authoring.RailSpacing,
            
            CarriageSizePlusPadding = authoring.CarriageSizePlusPadding
        });
    }
}