using Unity.Entities;

public struct MovingWall : IComponentData
{
    public float MoveSpeedInSeconds;
    public float MoveTimer;
    public int StartXIndex;
    public int CurrentXIndex;
    public bool MovingLeft;
    public int NumberOfTilesToMove;
}
