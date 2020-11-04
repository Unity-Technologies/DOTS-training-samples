using Unity.Entities;

[GenerateAuthoringComponent]
public struct Seat : IComponentData
{
    public bool IsAvailable;
    public Entity Carriage;
}