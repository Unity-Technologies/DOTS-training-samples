using Unity.Entities;
class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject RockPrefab;
    public int RockCount;
    public UnityEngine.GameObject RecepticalPrefab;
    public int RecepticalCount;


    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config
            {
                RockPrefab = GetEntity(authoring.RockPrefab),
                RockCount = authoring.RockCount,
                RecepticalPrefab = GetEntity(authoring.RecepticalPrefab),
                RecepticalCount = authoring.RecepticalCount
            });
        }
    }
}

struct Config : IComponentData
{
    public Entity RockPrefab;
    public int RockCount;
    public Entity RecepticalPrefab;
    public int RecepticalCount;

}