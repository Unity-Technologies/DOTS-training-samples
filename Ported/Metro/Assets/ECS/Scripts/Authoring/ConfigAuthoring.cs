using Unity.Entities;
using UnityEngine;

class ConfigAuthoring : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject CarriagePrefab;
    public GameObject PlatformPrefab;
    public GameObject CommuterPefab;
    public GameObject RailPrefab;
    
    public int TrainsToSpawn;
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
        });
    }
}