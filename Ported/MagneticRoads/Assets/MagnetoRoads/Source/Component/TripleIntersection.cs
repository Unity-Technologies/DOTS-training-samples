using Unity.Entities;

[GenerateAuthoringComponent]
public struct TripleIntersection : IComponentData
{
    public Entity laneIn0;
    public Entity laneIn1;
    public Entity laneIn2;
    public Entity laneOut0;
    public Entity laneOut1;
    public Entity laneOut2;
    public int carIndex;
    public int lane0Direction;
    public int lane1Direction;
    public int lane2Direction;
    public Entity car;
}
