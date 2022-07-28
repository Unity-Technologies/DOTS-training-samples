using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public int InitialFireCount;
    public float FireThreshold;
    public float HeatTransferRate;
    public int FireFighterLinesCount;
    public int FireFighterPerLineCount;

}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            InitialFireCount = authoring.InitialFireCount,
            FireThreshold = authoring.FireThreshold,
            HeatTransferRate = authoring.HeatTransferRate,  
            FireFighterLinesCount = authoring.FireFighterLinesCount,
            FireFighterPerLineCount = authoring.FireFighterPerLineCount
        });
    }
}