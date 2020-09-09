using Unity.Entities;

struct Tile : IComponentData
{
    [System.Flags]
    public enum Attributes : byte
    {
        None,
        
        Left = 1,
        Up = 2,
        Right = 4,
        Down = 8,
        
        Hole = 16,
        Goal = 32,
        Arrow = 64,
    }

    public Attributes Value;
}
