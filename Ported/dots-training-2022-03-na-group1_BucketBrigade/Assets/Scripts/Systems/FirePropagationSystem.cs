using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using UnityEngine;

public partial class FirePropagationSystem : SystemBase
{
    public const float HEAT_THRESHOLD = 0.2f;
    
    private NativeArray<int2> checkAdjacents;

    protected override void OnCreate()
    {
        //radius 1 -> 8  : 3*3-1
        //radius 2 -> 24 : 5*5-1 : (r+r+1)^2-1
        
        int radius = 2;
        int diameter = (2 * radius + 1);
        int arrayLength = diameter * diameter - 1;
        
        checkAdjacents = new NativeArray<int2>(arrayLength, Allocator.Persistent);

        int i = 0;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                
                checkAdjacents[i] = new int2(x,y);
                i++;
            }
        }
    }

    protected override void OnDestroy()
    {
        checkAdjacents.Dispose();
    }

    protected override void OnUpdate()
    {
        var heatmapData = GetSingleton<HeatMapData>(); 
        
        var heatmap = GetSingletonEntity<HeatMapTemperature>();
        DynamicBuffer<HeatMapTemperature> heatmapBuffer = EntityManager.GetBuffer<HeatMapTemperature>(heatmap);
        
        float deltaTime = Time.DeltaTime * heatmapData.heatPropagationSpeed;
        
        var buffer = heatmapBuffer.Reinterpret<float>();
        var localCheckAdjacents = checkAdjacents;

        Job.WithCode(() => {

            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] >= HEAT_THRESHOLD)
                {
                    buffer[i] += deltaTime;
                    HeatAdjacents(ref heatmapBuffer, localCheckAdjacents, i, heatmapData.mapSideLength, deltaTime);
                }

                if (buffer[i] >= 1f)
                    buffer[i] = 1f;
            }

        }).Run();

        var heatmapBufferReadonly = EntityManager.GetBuffer<HeatMapTemperature>(heatmap, true);

        //float dt = Time.DeltaTime * 10f;
        float time = (float) Time.ElapsedTime;
        float flickerRate = 1f;
        float flickerRange = 0.5f;
        
        Entities.WithAll<FireIndex>()
             .WithReadOnly(heatmapBufferReadonly)
             .ForEach( (ref URPMaterialPropertyBaseColor colorComponent, ref Translation translation, in FireIndex fireIndex) =>
             {
                 float intensity = heatmapBufferReadonly[fireIndex.index];

                 if (intensity < HEAT_THRESHOLD)
                 {
                     colorComponent.Value = heatmapData.colorNeutral;
                 }
                 else
                 {
                     colorComponent.Value = math.lerp(heatmapData.colorCool, heatmapData.colorHot, intensity);
                     translation.Value.y = math.lerp(0f, heatmapData.maxTileHeight, intensity);
                     
                     //Apply flicker noise
                     float2 sample = new float2((time - fireIndex.index) * flickerRate - intensity, intensity);
                     translation.Value.y += flickerRange * 0.5f + (noise.cnoise(sample) * flickerRange);
                 }


             })
             .ScheduleParallel();
    }

    static void HeatAdjacents(ref DynamicBuffer<HeatMapTemperature> buffer, NativeArray<int2> checkAdjacents, int tileIndex, int width, float deltaTime)
    {
        for (int iCheck = 0; iCheck < checkAdjacents.Length; iCheck++)
        {
            int2 tileCoord = GetTileCoordinate(tileIndex, width);
            int x = tileCoord.x + checkAdjacents[iCheck].x;
            int z = tileCoord.y + checkAdjacents[iCheck].y;

            bool inBounds = (x >= 0 && x <= width-1 && z >= 0 && z <= width-1);

            if (inBounds)
            {
                int adjacentIndex = GetTileIndex(x, z, width);
                
                if(buffer[adjacentIndex] < HEAT_THRESHOLD)
                    buffer[adjacentIndex] += deltaTime;
            }
        }
    }

   static int GetTileIndex(int x, int z, int width)
    {
        return (z * width) + x;
    }
    
    static int2 GetTileCoordinate(int index , int width)
    {
        int x = index / width;
        int z = index % width;
        return new int2(x, z);
    }
}
