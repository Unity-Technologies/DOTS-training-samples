using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerInput : IComponentData
{
    public int Speed;
    public bool NeedsReset;
}