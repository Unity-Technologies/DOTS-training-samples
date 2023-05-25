using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public static class SystemUtilities
{
    [BurstCompile]
    public static void PutoutFire(in float2 centerPos, in GameSettings settings, ref DynamicBuffer<FireTemperature> temperatures)
    {
        var gridSize = settings.DefaultGridSize;
        var cols = settings.RowsAndColumns;
        var gridPos = new int2((int)math.round(centerPos.x / gridSize), (int)math.round(centerPos.y / gridSize));
        PutoutFire(in gridPos, ref temperatures, cols, settings.PutOutSize);
    }
 
    [BurstCompile]
    static void PutoutFire(in int2 gridPos, ref DynamicBuffer<FireTemperature> temperatures, int cols, int putOutSize)
    {
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
    
    [BurstCompile]
    public static void GetNearestFirePosition(in float2 currentPosition, in GameSettings settings, in DynamicBuffer<FireTemperature> temperatures, out float2 closestPos)
    {
        var cols = settings.RowsAndColumns;
        var size = settings.Size;

        closestPos = float2.zero;
        var closestDist = float.MaxValue;
        
        for (var i = 0; i < size; i++)
        {
            if (temperatures[i] <= 0f) continue;
            
            var firePosition = new float2((i % cols) * settings.DefaultGridSize, (i / cols) * settings.DefaultGridSize);
            var dist = math.distancesq(currentPosition, firePosition);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPos = firePosition;
            }
        }
    } 
    
  
    [BurstCompile]
    public static void GetNearestWaterPosition(in float2 currentPosition, in GameSettings settings, in EntityQuery query, out float2 closestPos)
    {
        var transforms = query.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
        closestPos = float2.zero;
        var closestDist = float.MaxValue;

        for (var i = 0; i < transforms.Length; i++)
        {
            var waterPosition = transforms[i].Position.xz;
            var dist = math.distancesq(currentPosition, waterPosition);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPos = waterPosition;
            }
        }
    }     
}
