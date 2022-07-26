using Unity.Entities;

#region step1
class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject CarPrefab;
    public int CarCount;
    public float BrakingDistanceThreshold = 0.5f;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            CarPrefab = GetEntity(authoring.CarPrefab),
            CarCount = authoring.CarCount,
            BrakingDistanceThreshold = authoring.BrakingDistanceThreshold
        });
    }
}
#endregion