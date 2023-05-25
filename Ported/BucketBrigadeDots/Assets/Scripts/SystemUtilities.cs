using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

public static class SystemUtilities
{
    [BurstCompile]
    public static void PutoutFire(in float2 centerPos, in GameSettings settings, ref DynamicBuffer<FireTemperature> temperatures)
    {
        var gridSize = settings.DefaultGridSize;
        var cols = settings.RowsAndColumns;
        var gridPos = new int2((int)math.round(centerPos.x / gridSize), (int)math.round(centerPos.y / gridSize));
        PutoutFire(in gridPos, ref temperatures, cols);
    }
 
    [BurstCompile]
    static void PutoutFire(in int2 gridPos, ref DynamicBuffer<FireTemperature> temperatures, int cols)
    {
        var putOutSize = 10;
        
        for (var yD = ((putOutSize / 2) * -1); yD < (putOutSize / 2); yD++)
        {
            for (var xD = ((putOutSize / 2) * -1); xD < (putOutSize / 2); xD++)
            {
                var x = gridPos.x + xD;
                var y = gridPos.y + yD;
                if (x >= 0 && x < cols && y >= 0 && y < cols)
                {
                    var index = y * cols + x;
                    temperatures[index] = 0f;
                }
            }
        }
    }
}
