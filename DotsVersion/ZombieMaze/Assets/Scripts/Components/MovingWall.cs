using Unity.Entities;

public struct MovingWall : IComponentData
{
    public float MoveSpeedInSeconds;
    public float MoveTimer;
    public int StartXIndex;
    public int StartYIndex;
    public int CurrentXIndex;
    public int NumberOfTilesToMove;
    public bool MovingLeft;
}
