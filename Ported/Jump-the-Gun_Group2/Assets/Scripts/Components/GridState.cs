using Unity.Entities;
using Unity.Mathematics;

public struct GridHeight : IBufferElementData
{
    public float Height;
}

public struct GridOccupied : IBufferElementData
{
    public bool Occupied; // true if there is a cannon.
}

public struct GridTag : IComponentData
{}

public class GridFunctions
{
    public static int GetGridIndex(float2 xzPosition, int2 terrainDimension)
    {
        int2 tilePosition = math.int2(xzPosition);
        tilePosition.x = math.min(tilePosition.x, terrainDimension.x - 1);
        tilePosition.x = math.max(tilePosition.x, 0);
        tilePosition.y = math.min(tilePosition.y, terrainDimension.y - 1);
        tilePosition.y = math.max(tilePosition.y, 0);

        return tilePosition.y * terrainDimension.x + tilePosition.x;
    }
}

