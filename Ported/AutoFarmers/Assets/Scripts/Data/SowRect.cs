using Unity.Entities;

// This is basicaly just a rect, so maybe we should rename and merge it later with the TillRect
public struct SowRect : IComponentData
{
    public int X;
    public int Y;
    public int Width;
    public int Height;
}
