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
}