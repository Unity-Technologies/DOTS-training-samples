using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

readonly partial struct PlotAspect : IAspect
{
    public const int MAX_TILL = 100;

    public readonly Entity Self;

    public readonly TransformAspect Transform;

    public readonly RefRW<Plot> Plot;

    public void PlantSeed()
    {
        Plot.ValueRW.HasSeed = true;
    }

    public void GrowSeed(Entity plant)
    {
        Plot.ValueRW.HasPlant = true;
    }

    public void Harvest()
    {
        Plot.ValueRW.HasPlant = false;
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
