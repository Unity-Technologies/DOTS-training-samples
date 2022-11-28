using Unity.Entities;
using Unity.Rendering;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject WallPrefab;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config{ WallPrefab = GetEntity(authoring.WallPrefab)});
    }
}