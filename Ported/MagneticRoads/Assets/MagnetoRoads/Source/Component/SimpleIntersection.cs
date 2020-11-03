using Unity.Entities;

[GenerateAuthoringComponent]
public struct SimpleIntersection : IComponentData
{
    public Entity laneIn0;
    public Entity laneOut0;
    public Entity car;
}
