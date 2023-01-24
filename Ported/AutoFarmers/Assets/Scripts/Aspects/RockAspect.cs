using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

readonly partial struct RockAspect : IAspect
{
    public readonly Entity Self;

    public readonly TransformAspect Transform;

    public readonly RefRW<Rock> Rock;

    public int Health
    {
        get => Rock.ValueRW.RockHealth;
        set => Rock.ValueRW.RockHealth = value;
    }
}
