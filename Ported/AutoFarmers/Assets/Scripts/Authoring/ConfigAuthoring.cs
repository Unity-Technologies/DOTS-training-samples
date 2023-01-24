using Unity.Entities;
class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject RockPrefab;
    public int RockCount;
    public UnityEngine.GameObject SiloPrefab;
    public int SiloCount;
    public int GroundSizeXY;


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
                GroundSizeXY = authoring.GroundSizeXY
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
}