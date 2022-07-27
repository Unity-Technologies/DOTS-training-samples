using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public int GridSize;
    public float FireThreshold;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            GridSize = authoring.GridSize,
            FireThreshold = authoring.FireThreshold
        });
    }
}