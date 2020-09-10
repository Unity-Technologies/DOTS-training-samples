using Unity.Entities;

[GenerateAuthoringComponent]
public struct Arrow : IComponentData
{
    public Entity Owner;
    public Entity Tile;
}

public struct FreshArrowTag : IComponentData
{
}