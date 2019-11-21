using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MapGeneration
{
    public static IEnumerable<float2> GenerateObstaclePositions(int mapSize, int obstacleRingCount, float obstaclesPerRing, float obstacleRadius)
    {
        for (int i = 1; i <= obstacleRingCount; i++)
        {
            float ringRadius = (i / (obstacleRingCount + 1f)) * (mapSize * .5f);
            float circumference = ringRadius * 2f * Mathf.PI;
            int maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius) * 2f);
            int offset = Random.Range(0, maxCount);
            int holeCount = Random.Range(1, 3);
            for (int j = 0; j < maxCount; j++)
            {
                float t = (float)j / maxCount;
                if ((t * holeCount) % 1f < obstaclesPerRing)
                {
                    float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
                    yield return new float2(
                        mapSize * .5f + Mathf.Cos(angle) * ringRadius,
                        mapSize * .5f + Mathf.Sin(angle) * ringRadius);
                }
            }
        }
    }
}
