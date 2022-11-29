using Unity.Entities;

struct Config : IComponentData
{
    public Entity PersonPrefab;
    public int PersonCount;
    public float SafeZoneRadius;
}