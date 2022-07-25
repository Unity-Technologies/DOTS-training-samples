using Unity.Entities;

struct Config : IComponentData
{
    public Entity BlueBeePrefab;
    public Entity YellowBeePrefab;
    public int TeamBlueBeeCount;
    public int TeamYellowBeeCount;
    public float SafeZoneRadius;
}