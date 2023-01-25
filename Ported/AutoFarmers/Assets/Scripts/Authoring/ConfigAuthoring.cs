using Unity.Entities;
class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject RockPrefab;
    public int RockCount;
    public UnityEngine.GameObject SiloPrefab;
    public int SiloCount;
    public UnityEngine.GameObject FarmerPrefab;
    public UnityEngine.GameObject DronePrefab;
    public float SafeZoneRadius = 10.0f;


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
                safeZoneRadius = authoring.SafeZoneRadius
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
}