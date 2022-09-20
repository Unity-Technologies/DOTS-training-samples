using Unity.Entities;

#region step1
class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject TankPrefab;
    public int TankCount;
    public float SafeZoneRadius;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            TankPrefab = GetEntity(authoring.TankPrefab),
            TankCount = authoring.TankCount,
            SafeZoneRadius = authoring.SafeZoneRadius
        });
    }
}
#endregion