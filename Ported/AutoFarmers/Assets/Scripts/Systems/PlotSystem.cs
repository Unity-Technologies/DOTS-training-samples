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
            if ( plot.HasSeed() && !plot.HasPlant() )
            {
                //UnityEngine.Debug.Log("Seed is growing..");

                //randomize color of plant
                var hue = random.NextFloat();

                URPMaterialPropertyBaseColor RandomColor()
                {
                    hue = (hue + 0.618034005f) % 1;
                    var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
                    return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
                }

                var plant = state.EntityManager.Instantiate(config.PlantPrefab);
                state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(plant, RandomColor());


                var plantTransform = LocalTransform.FromPosition(plot.Transform.WorldPosition);
                state.EntityManager.SetComponentData<LocalTransform>(plant, plantTransform);

                var plantAspect = SystemAPI.GetAspectRW<PlantAspect>(plant);
                plantAspect.TimeToGrow = random.NextFloat(5.0f, 10.0f);
                //var plantAspect = state.EntityManager.GetAspect<PlantAspect>(plant);
                plantAspect.AssignPlot(plot.Self, (float)SystemAPI.Time.ElapsedTime);
                plot.GrowSeed(plant);
            }
        }

    }

}

