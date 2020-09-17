using Unity.Entities;
using Unity.Mathematics;

public enum Player
{
    Black,
    Red,
    Green,
    Blue
}

public struct ArrowPlacementRequest : IComponentData
{
    public DirectionEnum direction;
    public Player player;
    public int2 position;
    public bool remove;
}
