using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerComponent : IComponentData
{
    public int  PlayerId;
}

public struct LocalPlayerComponent : IComponentData
{
    public int  PlayerId;
}
