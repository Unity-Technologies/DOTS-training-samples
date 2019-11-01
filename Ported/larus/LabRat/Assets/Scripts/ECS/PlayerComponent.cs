using Unity.Entities;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct PlayerComponent : IComponentData
{
    [GhostDefaultField]
    public int  PlayerId;
}

public struct LocalPlayerComponent : IComponentData
{
    public int  PlayerId;
}
