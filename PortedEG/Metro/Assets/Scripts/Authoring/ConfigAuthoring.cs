using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject CommuterPrefab;

    public UnityEngine.GameObject PlatformPrefab;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            CommuterPrefab = GetEntity(authoring.CommuterPrefab),
            PlatformPrefab = GetEntity(authoring.PlatformPrefab)
        });
    }
}