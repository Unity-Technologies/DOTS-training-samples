using Unity.Entities;

struct Config : IComponentData
{
    public Entity AntPrefab;
    public int Amount;
}
