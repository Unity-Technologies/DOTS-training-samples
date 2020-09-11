using Unity.Entities;

struct Tile : IComponentData
{
    [System.Flags]
    public enum Attributes : ushort
    {
        None,

        WallLeft = 1 << 0,
        WallUp = 1 << 1,
        WallRight = 1 << 2,
        WallDown = 1 << 3,
        WallAny = WallLeft | WallUp | WallRight | WallDown,

        ArrowLeft = 1 << 4,
        ArrowUp = 1 << 5,
        ArrowRight = 1 << 6,
        ArrowDown = 1 << 7,
        ArrowAny = ArrowLeft | ArrowUp | ArrowRight | ArrowDown,
        ArrowShiftCount = 4,

        Hole = 1 << 8,
        Goal = 1 << 9,
        ObstacleAny = Hole | Goal,

        Spawn = 1 << 11,
    }

    public Attributes Value;
}

struct TileOwner : IComponentData
{
    public Entity Value;
}
