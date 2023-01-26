using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

readonly partial struct PlotAspect : IAspect
{
    public const int MAX_TILL = 100;

    public readonly Entity Self;

    public readonly TransformAspect Transform;

    public readonly RefRW<Plot> Plot;

    public Entity Plant
    {
        get => Plot.ValueRW.Plant;
        set => Plot.ValueRW.Plant = value;
    }

    public int2 PlotLocInWorld
    {
        get => Plot.ValueRW.PlotLocInWorld;
        set => Plot.ValueRW.PlotLocInWorld = value;
    }


    public void PlantSeed(int2 location)
    {
        Plot.ValueRW.HasSeed = true;
        PlotLocInWorld = location;
    }


    public void GrowSeed(Entity plant)
    {
        Plot.ValueRW.HasPlant = true;
        Plant = plant;
    }

    public void Harvest()
    {
        Plot.ValueRW.HasPlant = false;
        Plant = Entity.Null;
        Plot.ValueRW.HasSeed = false;
    }

    public bool HasSeed()
    {
        return Plot.ValueRW.HasSeed;
    }

    public bool HasPlant()
    {
        return Plot.ValueRW.HasPlant;
    }
}
