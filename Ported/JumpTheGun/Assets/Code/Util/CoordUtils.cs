using Unity.Mathematics;

public static class CoordUtils
{
    public static int2 ToCoords(int index, int width, int height)
    {
        return new int2(index % width, index / width);
    }
    
    public static int ToIndex(int2 coords, int width, int height)
    {
        return coords.x + coords.y * width;
    }
}
