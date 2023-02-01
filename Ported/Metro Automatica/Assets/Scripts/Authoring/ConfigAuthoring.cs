using Unity.Entities;
using UnityEngine;

class ConfigAuthoring : MonoBehaviour
{
    public GameObject StationPrefab;
    public int StationCount;
    public float SafeZoneRadius;
    public GameObject RailPrefab;
    public GameObject WagonPrefab;
    public GameObject HumanPrefab;
    public int WagonCount;
    public int HumanCount;

    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config
            {
                StationPrefab = GetEntity(authoring.StationPrefab),
                RailPrefab = GetEntity(authoring.RailPrefab),
                WagonPrefab = GetEntity(authoring.WagonPrefab),
                HumanPrefab = GetEntity(authoring.HumanPrefab),
                StationCount = authoring.StationCount,
                WagonCount = authoring.WagonCount,
                SafeZoneRadius = authoring.SafeZoneRadius,
                HumanCount = authoring.HumanCount
            });
        }
    }
}    

struct Config : IComponentData
{
    public Entity StationPrefab;
    public Entity RailPrefab;
    public Entity WagonPrefab;
    public Entity HumanPrefab;
    public int WagonCount;
    public int StationCount;
    public float SafeZoneRadius;
    public int HumanCount;
}
