using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject PersonPrefab;
    public int PersonCount;
    public float SafeZoneRadius;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            PersonPrefab = GetEntity(authoring.PersonPrefab),
            PersonCount = authoring.PersonCount,
            SafeZoneRadius = authoring.SafeZoneRadius
        });
    }
}