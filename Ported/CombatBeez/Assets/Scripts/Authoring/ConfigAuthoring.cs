using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject BlueBeePrefab;
    public UnityEngine.GameObject YellowBeePrefab;
    public int TeamBlueBeeCount;
    public int TeamYellowBeeCount;
    public float SafeZoneRadius;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            BlueBeePrefab = GetEntity(authoring.BlueBeePrefab),
            YellowBeePrefab = GetEntity(authoring.YellowBeePrefab),
            TeamBlueBeeCount = authoring.TeamBlueBeeCount,
            TeamYellowBeeCount = authoring.TeamYellowBeeCount,
            SafeZoneRadius = authoring.SafeZoneRadius
        });
    }
}