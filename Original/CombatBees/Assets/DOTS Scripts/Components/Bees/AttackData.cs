using Unity.Entities; 

[GenerateAuthoringComponent]
public struct AttackData : IComponentData
{
    public float Force;
    public float Distance;
}