using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using System.Diagnostics;
using Unity.Rendering;
using Unity.Mathematics;
using System;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct PlotSystem : ISystem
{
    float totalTime;
    Random random;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        totalTime = 0;
        random = Random.CreateFromIndex(1234);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        totalTime += SystemAPI.Time.DeltaTime;

        //Debug tiller
        if (totalTime > 1)
        {
            foreach( var plot in SystemAPI.Query<PlotAspect>() )
            {
                plot.Till(random.NextInt(5, 15));
            }
            totalTime = 0;
        }
        
        //Update the material once the plot is completely tilled
        //TODO have different materials to indicate progress
        foreach( var plot in SystemAPI.Query<PlotAspect>() )
        {
            if (plot.IsTilled())
            {
                ecb.SetComponent(plot.Self, TillColor());
            }
        }

    }

    URPMaterialPropertyBaseColor TillColor()
    {
        var color = new UnityEngine.Color(94.0f, 49.0f, 0.0f);
        return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4) color };
    }

}

