using Unity.Entities;
using Unity.Mathematics;
 
[GenerateAuthoringComponent]
public struct Segment : IComponentData
{
    float3 Start;
    float3 End;
}
