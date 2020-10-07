using Unity.Entities;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct RoadNode : IComponentData
{
     public Entity nodePosition;
     public int colorBits;
}