using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct NormalisedMoveTime : IComponentData
{
    public float Value;
}
