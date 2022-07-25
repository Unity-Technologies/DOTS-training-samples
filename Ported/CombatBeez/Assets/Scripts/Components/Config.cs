using Unity.Entities;

struct Config : IComponentData
{
    public Entity BeePrefab;
    public int TeamRedBeeCount;
    public int TeamBlueBeeCount;
    public float SafeZoneRadius;
}