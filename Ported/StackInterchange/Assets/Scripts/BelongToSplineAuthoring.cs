using Unity.Entities;
 
[GenerateAuthoringComponent]
public struct BelongToSpline : IComponentData
{
    public Entity Value;
}
