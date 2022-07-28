using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject BlueBeePrefab;
    public UnityEngine.GameObject YellowBeePrefab;
    public UnityEngine.GameObject FoodResourcePrefab;
    public UnityEngine.GameObject BloodPrefab;

    public int TeamBlueBeeCount;
    public int TeamYellowBeeCount;
    public int FoodResourceCount;

    public float FoodResourceDropRatePerSecond;
    public float FallingSpeed;
    public float BeeSpeed;
    public float AttackChance;

    public uint RandomNumberSeed;
    public int MinNumberOfBeesSpawned;
    public int MaxNumberOfBeesSpawned;
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
            BloodPrefab = GetEntity(authoring.BloodPrefab),
            TeamBlueBeeCount = authoring.TeamBlueBeeCount,
            TeamYellowBeeCount = authoring.TeamYellowBeeCount,
            FoodResourceCount = authoring.FoodResourceCount,
            FoodResourceDropRatePerSecond = authoring.FoodResourceDropRatePerSecond,
            FallingSpeed = authoring.FallingSpeed,
            BeeSpeed = authoring.BeeSpeed,
            AttackChance = authoring.AttackChance,
            RandomNumberSeed = authoring.RandomNumberSeed,
            MinNumberOfBeesSpawned = authoring.MinNumberOfBeesSpawned,
            MaxNumberOfBeesSpawned = authoring.MaxNumberOfBeesSpawned
        });
    }
}