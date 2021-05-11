using System.Collections.Generic;

using Unity.Collections;
using Unity.Mathematics;

public static class PlatformGenerator
{
    public enum PlatformType
    {
        Tank,
        Empty
    }

    public static NativeArray<PlatformType> CreatePlatforms(int width, int height, int2 playerPosition, int numberOfTanks, Allocator allocator = Allocator.Persistent)
    {
        int cellSizes = width * height;
        int tanksPlaced = 0;
        float tankChance = (float)numberOfTanks / (float)cellSizes;
        var random = new System.Random();
        int cellId = 0;

        numberOfTanks = math.min(numberOfTanks, cellSizes);

        var platforms = new NativeArray<PlatformType>(width * height, allocator);
        for (; cellId < cellSizes; ++cellId)
        {
            platforms[cellId] = PlatformType.Empty;
        }

        cellId = 0;
        while (tanksPlaced < numberOfTanks)
        {
            float randomVal = (float)random.NextDouble();
            
            int2 cellCoord = CoordUtils.ToCoords(cellId, width, height);
            bool isCellValid = cellCoord.x != playerPosition.x || cellCoord.y != playerPosition.y && platforms[cellId] == PlatformType.Empty;
            if (isCellValid && randomVal <= tankChance)
            {
                ++tanksPlaced;
                platforms[cellId] = PlatformType.Tank;
            }

            cellId = (cellId + 1) % cellSizes;
        }

        return platforms;
    }

}
