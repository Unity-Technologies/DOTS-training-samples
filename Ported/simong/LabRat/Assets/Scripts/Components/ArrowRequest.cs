using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ArrowRequest : IComponentData
{
    public int2 Position;
    public int OwnerID;
    public GridDirection Direction;
}
