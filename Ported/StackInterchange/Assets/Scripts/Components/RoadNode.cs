using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct RoadNode : IComponentData
{
     public float4 color;

     public Entity nextNode;
     public Entity exitNode;
}