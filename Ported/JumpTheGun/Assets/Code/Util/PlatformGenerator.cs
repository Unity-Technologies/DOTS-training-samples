using Unity.Collections;
using Unity.Mathematics;

public static class PlatformGenerator
{
    public enum PlatformType
    {
        Tank,
        Empty
    }

    public static NativeArray<PlatformType> CreatePlatforms(int width, int height, int2 playerPosition, int numberOfTanks)
    {
        int cellSizes = width * height;
        var platforms = new PlatformType[cellSizes];
        int tanksPlaced = 0;
        float tankChance = (float)numberOfTanks / (float)cellSizes;
        var random = new System.Random();
        for (int cellId = 0; cellId < cellSizes || tanksPlaced >= numberOfTanks; ++cellId)
        {
            float randomVal = (float)random.NextDouble();
            
            int2 cellCoord = CoordUtils.ToCoords(cellId, width, height);
            
            if (randomVal <= tankChance && cellCoord.x != playerPosition.x && cellCoord.y != playerPosition.y)
            {
                ++tanksPlaced;
                platforms[cellId] = PlatformType.Tank;
            }
            else
            {
                platforms[cellId] = PlatformType.Empty;
            }
        }
        return new NativeArray<PlatformType>(platforms, Allocator.Temp);
    }

}
