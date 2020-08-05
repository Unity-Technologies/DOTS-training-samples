using Unity.Entities;
using Unity.Mathematics;
 
[GenerateAuthoringComponent]
public struct Segment : IComponentData
{
    public float3 Start;
    public float3 End;
}
