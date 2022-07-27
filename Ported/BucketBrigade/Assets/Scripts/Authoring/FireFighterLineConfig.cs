using Unity.Entities;

struct FireFighterLineConfig : IComponentData
{
    public Entity Prefab;
    public int Count;

    public int FireFightersPerLine;
}
