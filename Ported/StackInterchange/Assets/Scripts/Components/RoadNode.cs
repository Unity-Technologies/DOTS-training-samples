using Unity.Entities;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct RoadNode : IComponentData
{
     public int colorBits;

     public Entity nextNode;
     public Entity exitNode;
}