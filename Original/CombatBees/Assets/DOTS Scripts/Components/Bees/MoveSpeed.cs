using Unity.Entities; 

[GenerateAuthoringComponent]
public struct MoveSpeed : IComponentData
{
    public float Value;
}