using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// initializes an array pheromone values and maintain pheromone values.
/// The size of the array is resolution * resolution. The systems checks every ant each frame.
/// if the ant walks over a cell, this systems adds to pheromone value of that cell (array index).
/// it also reduce the value of each cell by a rate every frame to simulate pheromone fading. 
/// </summary>
public partial struct PheromoneSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PheromoneData>(); 
    }

    public void OnUpdate(ref SystemState state)
    {
        var environmentData = SystemAPI.GetSingleton<EnvironmentData>();
        
        var pheromoneData = SystemAPI.GetSingletonBuffer<PheromoneData>();

        //init pheromone array for the first time
        if (pheromoneData.Length == 0)
        {
            pheromoneData.Length = environmentData.resolution * environmentData.resolution;
            

            for (var i = 0; i < pheromoneData.Length; i++)
            {
                pheromoneData[i] = new PheromoneData();
            }
            
            Debug.Log($"Pheromone data initialized."); 
        }

        //Ants drop pheromones
        float worldWidth = environmentData.Extents.x;
        float worldHeight = environmentData.Extents.y;
        float3 rectMinPoint = environmentData.Center - 0.5f * new float3(worldWidth, 0f, worldHeight); //bottom-left of the enc rect.
        int res = environmentData.resolution;
        
        foreach ((MoveToPositionAspect moveToPositionAspect, TransformAspect transformAspect) in SystemAPI.Query<MoveToPositionAspect, TransformAspect>())
        {
            //Ants relative position in our env rect
            float3 antPosRelative = transformAspect.WorldPosition - rectMinPoint;
            
            // Debug.Log($"trying x = {worldWidth}, y = {worldHeight}, res = {res}, ant x ={antPosRelative.x}, ant z ={antPosRelative.z}");
            var cellIndex = GetCellIndex(worldWidth, worldHeight, res, antPosRelative.x, antPosRelative.z);

            //Add to pheromone value by DropRate
            var oldPheromoneData = pheromoneData[cellIndex];
            pheromoneData[cellIndex] = new PheromoneData()
            {
                    value = oldPheromoneData.value + environmentData.DropRate
            };
        }
        
        //pheromones fading
        for (var i = 0; i < pheromoneData.Length; i++)
        {
            //Subtract from pheromone value by FadeRate
            var oldData = pheromoneData[i];
            pheromoneData[i] = new PheromoneData()
            {
                    value = oldData.value - environmentData.FadeRate
            };

            if (pheromoneData[i].value < 0f)
                pheromoneData[i] = new PheromoneData();
        }
    }


    /// <summary>
    /// This utility method gets the bounds of the environment and the position of an ant
    /// and return the cell index the ant is in. x and y values must be calculated relative to the min of the rect.
    /// </summary>
    /// <param name="w"> width of the environment rectangle </param>
    /// <param name="h"> height of the environment rectangle </param>
    /// <param name="n"> number of cells (resolution) </param>
    /// <param name="x"> sampling position x </param>
    /// <param name="y"> sampling position y </param>
    /// <returns></returns>
    int GetCellIndex(float w, float h, int n, float x, float y)
    {
        if (x < 0f || x > w)
            throw new ArgumentOutOfRangeException($"{nameof(x)} must be a positive value less than {w}");
        if (y < 0f || y > h)
            throw new ArgumentOutOfRangeException($"{nameof(y)} must be a positive value less than {h}");
        
        float segmentWidth = w / n;
        float segmentHeight = h / n;

        int segmentX = (int)(x / segmentWidth);
        int segmentY = (int)(y / segmentHeight);

        int segmentNumber = segmentY * n + segmentX;

        // if (segmentNumber < 0)
        //     Debug.Log($"problem segmentWidth = {segmentWidth}, segmentHeight = {segmentHeight}, segmentX = {segmentX}, segmentY ={segmentY}, segmentNumber ={segmentNumber}");
        
        return segmentNumber;
    }
}
