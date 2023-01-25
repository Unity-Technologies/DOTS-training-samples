using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

readonly partial struct PlantAspect : IAspect
{
    public readonly Entity Self;

    public readonly TransformAspect Transform;

    public readonly RefRW<Plant> Plant;

    public Entity Plot
    {
        get => Plant.ValueRW.plot;
        set => Plant.ValueRW.plot = value;
    }
    public bool HasPlot
    {
        get => Plant.ValueRW.hasPlot;
        set => Plant.ValueRW.hasPlot = value;
    }
        
    public bool ReadyToPick
    {
        get => Plant.ValueRW.isReadyToPick;
        set => Plant.ValueRW.isReadyToPick = value;
    }

    public bool PickedAndHeld
    {
        get => Plant.ValueRW.pickedAndHeld;
        set => Plant.ValueRW.pickedAndHeld = value;
    }

    public bool BeingTargeted
    {
        get => Plant.ValueRW.beingTargeted;
        set => Plant.ValueRW.beingTargeted = value;
    }

    public float TimePlanted
    {
        get => Plant.ValueRW.timePlanted;
        set => Plant.ValueRW.timePlanted = value;
    }

    public void AssignPlot(Entity plot, float plantTime)
    {
        Plot = plot;
        HasPlot = true;
        TimePlanted = plantTime;
    }
}
