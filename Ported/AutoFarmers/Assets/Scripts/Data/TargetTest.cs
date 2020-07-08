using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargetTest : IComponentData
{
    public Entity TargetOne;
    public Entity TargetTwo;
    public Entity TargetThree;
}