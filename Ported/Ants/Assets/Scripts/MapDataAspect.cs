using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct MapDataAspect : IAspect
{
    public readonly RefRW<MapData> MapData;
    public int Rows => MapData.ValueRO.Rows;
    public int Columns => MapData.ValueRO.Columns;
    
    public float GetStrength(int row, int col)
    {
        return MapData.ValueRO.StrengthList[col * Rows + row] / 255f;
    }

    public void SetStrength(int row, int col, float value)
    {
        byte currentValue = MapData.ValueRO.StrengthList[col * Rows + row];
        byte nowValue = (byte)math.min(255, value);
        MapData.ValueRW.StrengthList[col * Rows + row] = nowValue;
    }
    
    public void AddStrength(int row, int col, float value, int radius = -1)
    {
        byte currentValue = MapData.ValueRO.StrengthList[col * Rows + row];
        byte nowValue = (byte)math.min(255, currentValue + value);
        MapData.ValueRW.StrengthList[col * Rows + row] = nowValue;
        
        //TODO - Integrate radius parameter...
        
    }

    public TileType GetTileType(int row, int col)
    {
        return MapData.ValueRO.TileTypes[col * Rows + row];
    }

    public void SetTile(int row, int col, TileType tileType, int radius = -1)
    {
        int tileIndex = col * Rows + row;
        MapData.ValueRW.TileTypes[tileIndex] = tileType;

        //TODO - Integrate radius parameter...
        /*
        for (int i = 0; i < radius; i++)
        {
            if(tileIndex - i * rows > 0)
                MapData.ValueRW.TileTypes[tileIndex - i * rows] = tileType;
        }
        */
    }
}
