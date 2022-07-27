using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject BlueBeePrefab;
    public UnityEngine.GameObject YellowBeePrefab;
    public UnityEngine.GameObject FoodResourcePrefab;
    public int TeamBlueBeeCount;
    public int TeamYellowBeeCount;
    public int FoodResourceCount;
    public float FoodResourceDropRatePerSecond;
    public float FallingSpeed;
    public float BeeSpeed;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            BlueBeePrefab = GetEntity(authoring.BlueBeePrefab),
            YellowBeePrefab = GetEntity(authoring.YellowBeePrefab),
            FoodResourcePrefab = GetEntity(authoring.FoodResourcePrefab),
            TeamBlueBeeCount = authoring.TeamBlueBeeCount,
            TeamYellowBeeCount = authoring.TeamYellowBeeCount,
            FoodResourceCount = authoring.FoodResourceCount,
            FoodResourceDropRatePerSecond = authoring.FoodResourceDropRatePerSecond,
            FallingSpeed = authoring.FallingSpeed,
            BeeSpeed = authoring.BeeSpeed
        });
    }
}