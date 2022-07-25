using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject BeePrefab;
    public int TeamRedBeeCount;
    public int TeamBlueBeeCount;
    public float SafeZoneRadius;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            BeePrefab = GetEntity(authoring.BeePrefab),
            TeamRedBeeCount = authoring.TeamRedBeeCount,
            TeamBlueBeeCount = authoring.TeamBlueBeeCount,
            SafeZoneRadius = authoring.SafeZoneRadius
        });
    }
}