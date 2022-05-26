using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

public static class MapUtil
{

    public static int MapCordToIndex(int2 mapSize, int2 cords)
    {
        return mapSize.x * cords.y + cords.x;
    }

    public static int MapCordToIndex(int2 mapSize, int x, int y)
    {
        return mapSize.x * y + x;
    }

}
