using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct CarryingFood : IComponentData
{
    public bool value;
}