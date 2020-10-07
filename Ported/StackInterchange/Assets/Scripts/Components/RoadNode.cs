using Unity.Entities;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct RoadNode : IComponentData
{
     public uint index;
     public Entity nodePosition;
     public int colorBits;
     
     public Entity nextNode;
}