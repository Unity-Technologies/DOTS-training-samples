using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Bucket : IComponentData
{
    public bool HasWater;
    public bool isHeld;
}