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
        var gridPos = GetGridPosition(in centerPos, gridSize);
        PutoutFire(in gridPos, ref temperatures, cols, settings.PutOutSize);
    }
    
    public static int2 GetGridPosition(in float2 position, in float gridSize)
    {
        return new int2((int)math.round(position.x / gridSize), (int)math.round(position.y / gridSize));
    }
    
    public static int GetGridIndex(in int2 gridPos, in int cols)
    {
        return gridPos.y * cols + gridPos.x;
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
                    var newGridPos = new int2(x, y);
                    var index = GetGridIndex(newGridPos, cols);
                    var dist = math.distance(gridPos, newGridPos);
                    var newTemp = math.lerp(0f, temperatures[index], dist / (float)putOutSize);
                    if (newTemp < 0.1f)
                        newTemp = 0f;
                    temperatures[index] = newTemp;
                }
            }
        }
    }
    
    [BurstCompile]
    public static void GetNearestFirePosition(in float2 currentPosition, in GameSettings settings, in DynamicBuffer<FireTemperature> temperatures, out float2 closestPos)
    {
        var cols = settings.RowsAndColumns;
        var size = settings.Size;

        closestPos = new float2(float.MinValue);
        var closestDist = float.MaxValue;

        var random = Random.CreateFromIndex((uint)currentPosition.GetHashCode());
        for (var i = 0; i < size; i++)
        {
            if (temperatures[i] <= 0f) continue;
            
            var firePosition = new float2((i % cols) * settings.DefaultGridSize, (i / cols) * settings.DefaultGridSize);
            var dist = math.distancesq(currentPosition, firePosition);
            dist += random.NextFloat(0, 10f);
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
        closestPos = new float2(float.MinValue);
        var closestDist = float.MaxValue;

        var random = Random.CreateFromIndex((uint)currentPosition.GetHashCode());
        for (var i = 0; i < transforms.Length; i++)
        {
            var waterPosition = transforms[i].Position.xz;
            var dist = math.distancesq(currentPosition, waterPosition);
            dist += random.NextFloat(0, 10f);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPos = waterPosition;
            }
        }
    }     
}
