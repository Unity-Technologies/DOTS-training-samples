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
}
