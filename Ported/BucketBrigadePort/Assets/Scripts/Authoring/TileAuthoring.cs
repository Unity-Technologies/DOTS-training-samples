using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Tile : IComponentData
{
    public int Id;
}

public struct OnFire : IComponentData { }
