using Unity.Entities; 

[GenerateAuthoringComponent]
public struct MoveTarget : IComponentData
{
    public Entity Value;
}