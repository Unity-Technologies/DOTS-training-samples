using Unity.Entities;
using Unity.Mathematics;
 
[GenerateAuthoringComponent]
public struct Size : IComponentData
{
    public float3 Value;
}
