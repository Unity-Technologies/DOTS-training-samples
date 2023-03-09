using Unity.Entities;

struct Config : IComponentData
{
    public Entity BlueBeePrefab;
    public Entity YellowBeePrefab;
    public Entity FoodResourcePrefab;
    public Entity BloodPrefab;

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

    public float TimeBloodTakesToDry;

    public bool Respawn;
}