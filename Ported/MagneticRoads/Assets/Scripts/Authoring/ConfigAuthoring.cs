using Unity.Entities;

#region step1
class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject CarPrefab;
    public int CarCount;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            CarPrefab = GetEntity(authoring.CarPrefab),
            CarCount = authoring.CarCount
        });
    }
}
#endregion