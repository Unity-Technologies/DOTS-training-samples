using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ResourceParamsSystem : SystemBase
{
    protected override void OnCreate()
    {
        /*
        var field = GetSingleton<FieldAuthoring>();
        var resParams = GetSingleton<ResourceParamsAuthoring>();

        int gridCountsX = (int)field.size.x / (int)resParams.resourceSize;
        int gridCountsZ = (int)field.size.z / (int)resParams.resourceSize;
        int2 gridCounts = new int2(gridCountsX, gridCountsZ);

        float gridSizeX = field.size.x / gridCounts.x;
        float gridSizeZ = field.size.z / gridCounts.y;
        float2 gridSize = new float2(gridSizeX, gridSizeZ);

        float minGridPosX = (gridCounts.x - 1f) * -.5f * gridSize.x;
        float minGridPosZ = (gridCounts.y - 1f) * -.5f * gridSize.y;
        float2 minGridPos = new float2(minGridPosX, minGridPosZ);
        int[,] stackHeights = new int[gridCounts.x, gridCounts.y];
        */

    }

    protected override void OnUpdate()
    {
        
    }
}