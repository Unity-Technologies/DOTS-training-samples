using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ArrowComponent : IComponentData
{
    public int2 GridCell;
    public int OwnerID;
    public double SpawnTime;
}
