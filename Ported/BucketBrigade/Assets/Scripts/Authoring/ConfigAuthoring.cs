using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public int GridSize;
    public int InitialFireCount;
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
            InitialFireCount = authoring.InitialFireCount,
            FireThreshold = authoring.FireThreshold,
            FireFighterLinesCount = authoring.FireFighterLinesCount,
            FireFighterPerLineCount = authoring.FireFighterPerLineCount
        });
    }
}