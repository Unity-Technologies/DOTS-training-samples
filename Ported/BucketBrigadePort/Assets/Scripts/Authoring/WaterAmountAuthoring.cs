using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct WaterAmount : IComponentData
{
    public float Value;
    public float MaxAmount;
}
public struct WaterRefill : IComponentData { }
