using Unity.Entities;
using UnityEngine;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public GameObject RockPrefab;
    public int RockCount;
    public GameObject SiloPrefab;
    public int SiloCount;
    public GameObject FarmerPrefab;
    public GameObject DronePrefab;
    public float SafeZoneRadius = 10.0f;
    public GameObject PlantPrefab;
    public GameObject PlotPrefab;


    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config
            {
                RockPrefab = GetEntity(authoring.RockPrefab),
                RockCount = authoring.RockCount,
                SiloPrefab = GetEntity(authoring.SiloPrefab),
                SiloCount = authoring.SiloCount,
                FarmerPrefab = GetEntity(authoring.FarmerPrefab),
                DronePrefab = GetEntity(authoring.DronePrefab),
                safeZoneRadius = authoring.SafeZoneRadius,
                PlantPrefab = GetEntity(authoring.PlantPrefab),
                PlotPrefab = GetEntity(authoring.PlotPrefab)
            });
        }
    }
}

struct Config : IComponentData
{
    public Entity RockPrefab;
    public int RockCount;
    public Entity SiloPrefab;
    public int SiloCount;
    public Entity FarmerPrefab;
    public Entity DronePrefab;
    public float safeZoneRadius;
    public Entity PlantPrefab;
    public Entity PlotPrefab;
}