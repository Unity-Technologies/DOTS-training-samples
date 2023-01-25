using Unity.Entities;
class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject RockPrefab;
    public int RockCount;
    public UnityEngine.GameObject SiloPrefab;
    public int SiloCount;
    public int GroundSizeXY;
    public UnityEngine.GameObject FarmerPrefab;
    public UnityEngine.GameObject DronePrefab;
    public UnityEngine.GameObject PlantPrefab;


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
                GroundSizeXY = authoring.GroundSizeXY,
                FarmerPrefab = GetEntity(authoring.FarmerPrefab),
                DronePrefab = GetEntity(authoring.DronePrefab),
                PlantPrefab = GetEntity(authoring.PlantPrefab)

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
    public int GroundSizeXY;
    public Entity FarmerPrefab;
    public Entity DronePrefab;
    public Entity PlantPrefab;
}