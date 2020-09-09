using Unity.Entities;

[GenerateAuthoringComponent]
public struct Player : IComponentData
{
    public Entity Arrow0;
    public Entity Arrow1;
    public Entity Arrow2; // Maybe all these could be some sort of fixed buffer?
    public int CurrentArrow;
}
