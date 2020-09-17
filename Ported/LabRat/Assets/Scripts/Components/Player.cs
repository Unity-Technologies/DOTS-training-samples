using Unity.Entities;

public enum Player
{
    Black,
    Red,
    Green,
    Blue
}

public struct PlayerId : IComponentData
{
    public Player Value;
}
