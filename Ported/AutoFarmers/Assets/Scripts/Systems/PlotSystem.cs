using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using System.Diagnostics;
using Unity.Rendering;
using Unity.Mathematics;
using System;
using Random = Unity.Mathematics.Random;
using Unity.Collections;

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
        state.RequireForUpdate<Config>();
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

        foreach( var plot in SystemAPI.Query<PlotAspect>() )
        {
            if (! plot.HasSeed() )
            {
                UnityEngine.Debug.Log("Seeding a plot..");
                plot.PlantSeed();
            }
        }
        
        foreach( var plot in SystemAPI.Query<PlotAspect>() )
        {
            if ( plot.HasSeed() && !plot.HasPlant() )
            {
                var plant = state.EntityManager.Instantiate(config.PlantPrefab);
                var plantTransform = LocalTransform.FromPosition(plot.Transform.WorldPosition);
                state.EntityManager.SetComponentData<LocalTransform>(plant, plantTransform);

                var plantAspect = state.EntityManager.GetAspect<PlantAspect>(plant);
                plantAspect.Plot = plot.Self;
                plantAspect.HasPlot = true;

                plot.GrowSeed(plant);
            }
        }

    }

}

