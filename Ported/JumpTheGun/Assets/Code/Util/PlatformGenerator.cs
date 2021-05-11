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

    public static void CreatePlatforms(int width, int height, int2 playerPosition, int numberOfTanks, out List<PlatformType> platforms, out List<int2> tankPositions)
    {
        int cellSizes = width * height;
        int tanksPlaced = 0;
        float tankChance = (float)numberOfTanks / (float)cellSizes;
        var random = new System.Random();
        platforms = new List<PlatformType>();
        tankPositions = new List<int2>();

        for (int cellId = 0; cellId < cellSizes || tanksPlaced >= numberOfTanks; ++cellId)
        {
            float randomVal = (float)random.NextDouble();
            
            int2 cellCoord = CoordUtils.ToCoords(cellId, width, height);

            PlatformType platformType;
            if (randomVal <= tankChance && cellCoord.x != playerPosition.x && cellCoord.y != playerPosition.y)
            {
                ++tanksPlaced;
                platformType = PlatformType.Tank;
                tankPositions.Add(cellCoord);
            }
            else
            {
                platformType = PlatformType.Empty;
            }

            platforms.Add(platformType);
        }
    }

}
