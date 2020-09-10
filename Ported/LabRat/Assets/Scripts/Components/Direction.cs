using Unity.Entities;

struct Direction : IComponentData
{
    [System.Flags]
    public enum Attributes : byte
    {
        None,

        Left   = (byte)Tile.Attributes.WallLeft,
        Up     = (byte)Tile.Attributes.WallUp,
        Right  = (byte)Tile.Attributes.WallRight,
        Down   = (byte)Tile.Attributes.WallDown,
        Any    = Left | Up | Right | Down,
    }

    public Attributes Value;
}
