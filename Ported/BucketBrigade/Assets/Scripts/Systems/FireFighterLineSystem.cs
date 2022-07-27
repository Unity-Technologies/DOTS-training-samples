using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
struct heatmm
{
    public NativeArray<float> HeatValues;
}

[BurstCompile]
partial struct FireFighterLineSystem : ISystem
{
    private heatmm heatMap;

    private int GridSize;

    private bool calculated;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Debug.Log("IN CREATE");
        heatMap.HeatValues =  new NativeArray<float>(4, Allocator.Temp)
        {

            [0] = 0f,

            [1] = 0f,

            [2] = 1f,

            [3] = 1f 

        };
        calculated = false;
        
        // Create Lines
        state.RequireForUpdate<Config>();
        /*var config = SystemAPI.GetSingleton<Config>();

        for (int i = 0; i < config.FireFighterLinesCount; i++)
        {
            for (int j = 0; j < config.FireFighterPerLineCount; j++)
            {
                
            }
        }*/

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // See note above regarding the [BurstCompile] attribute.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        foreach (var ffline in SystemAPI.Query<RefRW<FireFighterLine>>())
        {
            var closestPoint = new float2(999999, 999999);
            int index = 0;
            foreach (var heatValue in heatMap.HeatValues)
            {
                if (heatValue >= config.FireThreshold)
                {
                    var newPoint = new float2(index % config.GridSize, index / config.GridSize);
                    if (math.distance(ffline.ValueRO.StartPosition, closestPoint) >
                        math.distance(ffline.ValueRO.StartPosition, newPoint))
                    {
                        closestPoint = newPoint;
                    }
                }
                index++;
            }

            ffline.ValueRW.EndPosition = closestPoint;
            
            if (!calculated)
            {
                calculated = true;
                Debug.Log(ffline.ValueRW.EndPosition.ToString());
            }
        }
    }
}