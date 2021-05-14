using System.Collections.Generic;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

public static class PlatformGenerator
{
    public static NativeArray<int2> CreateTanks(int width, int height, int2 playerPosition, int numberOfTanks, Random random, ref DynamicBuffer<TankMap> tanks, Allocator allocator)
    {
        int cellSizes = width * height;
        int tanksPlaced = 0;
        float tankChance = (float)numberOfTanks / (float)cellSizes;
        int cellId = 0;

        NativeArray<int2> output = new NativeArray<int2>(numberOfTanks, allocator, NativeArrayOptions.UninitializedMemory);

        numberOfTanks = math.min(numberOfTanks, cellSizes);

        tanks.ResizeUninitialized(cellSizes);
        for (int i = 0; i < cellSizes; ++i)
            tanks[i] = new TankMap {Value = false};
        
        cellId = 0;
        while (tanksPlaced < numberOfTanks)
        {
            float randomVal = random.NextFloat();
            
            int2 cellCoord = CoordUtils.ToCoords(cellId, width, height);
            bool isCellValid = cellCoord.x != playerPosition.x || cellCoord.y != playerPosition.y && tanks[cellId].Value == false;
            if (isCellValid && randomVal <= tankChance)
            {
                output[tanksPlaced] = cellCoord;
                ++tanksPlaced;
                tanks[cellId] = new TankMap {Value = true};
            }

            cellId = (cellId + 1) % cellSizes;
        }
        return output;
    }
    
    
}
