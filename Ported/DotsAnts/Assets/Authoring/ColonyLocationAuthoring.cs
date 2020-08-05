using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ColonyLocation : IComponentData
{
  public float2 value;
}