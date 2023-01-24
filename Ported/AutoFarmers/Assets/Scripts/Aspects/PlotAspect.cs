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

    public void Till(int tillAmount)
    {
        Plot.ValueRW.TillStatus += tillAmount;
    }

    public bool IsTilled()
    {
        return Plot.ValueRW.TillStatus >= MAX_TILL;
    }

    public void StartPlant(Entity plant)
    {
        Plot.ValueRW.Plant = plant;
    }

    public void Harvest()
    {
        Plot.ValueRW.HasPlant = false;
        Plot.ValueRW.TillStatus = 0;
    }
}
