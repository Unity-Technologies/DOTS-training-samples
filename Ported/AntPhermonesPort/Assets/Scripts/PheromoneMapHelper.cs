using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PheromoneMapHelper
{
    float2 worldLowerLeft;
    float2 worldUpperRight;

    DynamicBuffer<PheromoneMap> pheromoneMap;
    int gridSize;

    public bool IsInitialized()
    {
        return pheromoneMap.Length > 0;
    }

    public PheromoneMapHelper(DynamicBuffer<PheromoneMap> _cellmap, int _gridSize, float _worldSize)
    {
        pheromoneMap = _cellmap;
        gridSize = _gridSize;

        // World centered at 0,0
        var halfSize = _worldSize / 2;
        worldLowerLeft = new float2(-halfSize, -halfSize);
        worldUpperRight = new float2(halfSize, halfSize);
    }

    public void InitPheromoneMap()
    {
        pheromoneMap.Length = gridSize * gridSize;

        // UNCOMMENT TO GET A TEXTURE TEST
        //for (int x = 0; x < gridSize / 2; x++)
        //{
        //    for (int y = 0; y < gridSize / 2; ++y)
        //    {
        //        pheromoneMap.ElementAt(y * gridSize + x).intensity = new float4(0, 0.2f, 0, 1);
        //    }
        //}

        //for (int x = gridSize / 2; x < gridSize; x++)
        //{
        //    for (int y = gridSize / 2; y < gridSize; ++y)
        //    {
        //        pheromoneMap.ElementAt(y * gridSize + x).intensity = new float4(0, 0.6f, 0, 1);
        //    }
        //}

        //// Optimize
        for (int i = 0; i < pheromoneMap.Length; ++i)
        {
            pheromoneMap.ElementAt(i).intensity = new float4(0, 0, 0, 1);
        }
    }

    /// <summary>
    /// Returns the nearest index to a point in world space. The originOffset is used
    /// to align the [0,0] index with the start of the grid (since the grid origin in
    /// worldspace might be [-5.0, -5.0] for example if the plane of size 5 is at [0,0])
    /// </summary>
    /// <param name="xy"></param>
    /// <param name="originOffset"></param>
    /// <returns></returns>
    public int GetNearestIndex(float2 xy)
    {
        if (xy.x < worldLowerLeft.x || xy.y < worldLowerLeft.y || xy.x > worldUpperRight.x || xy.y > worldUpperRight.y)
        {
            //TBD: Warnings are disabled currently because ant move might jump out past the boundary cell
            //Debug.LogError("[Cell Map] Trying to get index out of range");
            return -1;
        }

        float2 gridRelativeXY = xy - worldLowerLeft;

        float2 gridIndexScaleFactor = (worldUpperRight - worldLowerLeft) / gridSize;
        float2 gridIndexXY = gridRelativeXY / gridIndexScaleFactor;

        return ConvertGridIndex2Dto1D(gridIndexXY);
    }

    public float GetPheromoneIntensityFrom2DPos(float2 xy)
    {
        int index = GetNearestIndex(xy);
        if (index < 0 || index > pheromoneMap.Length)
        {
            //Debug.LogError(string.Format("[Cell Map] Position is outside cell map {0}, {1}", xy.x, xy.y));
            return -1;
        }

        return pheromoneMap[index].intensity.x;
    }

    public int ConvertGridIndex2Dto1D(float2 index)
        => Mathf.RoundToInt(index.y) * gridSize + Mathf.RoundToInt(index.x);

    public void Set(int x, int y, float state)
    {
        pheromoneMap.ElementAt(y * gridSize + x).intensity = state;
    }

    public float Get(int x, int y)
    {
        return pheromoneMap[y * gridSize + x].intensity.x;
    }

    public void IncrementIntensity(float2 position, float increaseValue)
    {
        int index = GetNearestIndex(position);
        float4 currentValue = pheromoneMap[index].intensity;

        pheromoneMap.ElementAt(index).intensity = new float4(
            math.clamp(currentValue.x + increaseValue, 0, 1), 0, 0, 1
        );
    }

    public void DecrementIntensity(float decreaseValue)
    {
        for(int i = 0; i<pheromoneMap.Length; i++)
        {
            pheromoneMap.ElementAt(i).intensity -= new float4(decreaseValue, 0, 0, 0);
        }
    }
}
