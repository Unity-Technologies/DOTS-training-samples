using Unity.Entities;
using Unity.Mathematics;
 
[GenerateAuthoringComponent]
public struct Size : IComponentData
{
    float3 Value;
}
