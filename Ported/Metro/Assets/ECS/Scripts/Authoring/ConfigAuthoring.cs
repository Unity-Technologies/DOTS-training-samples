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
    public float BEZIER_HANDLE_REACH = 0.1f;
    public float BEZIER_PLATFORM_OFFSET = 3f;
    public float PLATFORM_ADJACENCY_LIMIT = 12f;
    public int BEZIER_MEASUREMENT_SUBDIVISIONS = 2;
    public float PLATFORM_ARRIVAL_THRESHOLD = 0.975f;
    public float RAIL_SPACING = 0.5f;
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
            
            TrainsToSpawn = authoring.TrainsToSpawn,
            
            BEZIER_HANDLE_REACH = authoring.BEZIER_HANDLE_REACH,
            BEZIER_PLATFORM_OFFSET = authoring.BEZIER_PLATFORM_OFFSET,
            PLATFORM_ADJACENCY_LIMIT = authoring.PLATFORM_ADJACENCY_LIMIT,
            BEZIER_MEASUREMENT_SUBDIVISIONS = authoring.BEZIER_MEASUREMENT_SUBDIVISIONS,
            PLATFORM_ARRIVAL_THRESHOLD = authoring.PLATFORM_ARRIVAL_THRESHOLD,
            RAIL_SPACING = authoring.RAIL_SPACING
        });
    }
}