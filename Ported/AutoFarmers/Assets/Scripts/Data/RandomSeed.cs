using Unity.Entities;

[GenerateAuthoringComponent]
public struct RandomSeed : IComponentData
{
    public uint Value;
}