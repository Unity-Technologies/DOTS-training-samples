using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public int GridSize;
    public float FireThreshold;
    public int FireFighterLinesCount;
    public int FireFighterPerLineCount;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            GridSize = authoring.GridSize,
            FireThreshold = authoring.FireThreshold,
            FireFighterLinesCount = authoring.FireFighterLinesCount,
            FireFighterPerLineCount = authoring.FireFighterPerLineCount
        });
    }
}