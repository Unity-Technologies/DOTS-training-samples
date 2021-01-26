using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PheromoneMap : IComponentData
{
    public int Resolution;
    public float WorldSpaceSize;
    public float AntPheremoneStrength;
    public float PheremoneDecay;
    public float GoalSeekExcitementFactor;
    public float HasFoodExcitementFactor;

    public static int2 WorldToGridPos(PheromoneMap map, float3 worldPos)
    {
        float offset = map.WorldSpaceSize / 2f;
        float x = (worldPos.x + offset) / map.WorldSpaceSize;
        float z = (worldPos.z + offset) / map.WorldSpaceSize;
        return new int2((int)math.round(x * map.Resolution), (int)math.round(z * map.Resolution));
    }

    public static float3 GridToWorldPos(PheromoneMap map, int2 gridPos)
    {
        float offset = map.WorldSpaceSize / 2f;
        float x = ((gridPos.x) / (float)(map.Resolution));
        float z = ((gridPos.y) / (float)(map.Resolution));
        float3 worldSpacePos = new float3(x * map.WorldSpaceSize, 0, z * map.WorldSpaceSize);
        worldSpacePos.x -= offset;
        worldSpacePos.z -= offset;
        return worldSpacePos;
    }

    public static int GridPosToIndex(PheromoneMap map, int2 gridPos)
    {
        var index = gridPos.x + map.Resolution * gridPos.y;
        index = math.clamp(index, 0, (map.Resolution * map.Resolution) - 1);
        return index;
    }
}
