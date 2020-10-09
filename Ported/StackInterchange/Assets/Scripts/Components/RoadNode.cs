using Assets.Scripts.BlobData;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct RoadNode : IComponentData
{
     public ColorMask.Masks colorMask;
     public Entity nextNode;
     public Entity exitNode;
}