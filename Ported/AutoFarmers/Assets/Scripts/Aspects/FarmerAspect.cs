using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

readonly partial struct FarmerAspect : IAspect
{
    public readonly Entity Self;

    public readonly TransformAspect Transform;

    public readonly RefRW<Farmer> Farmer;

    public byte FarmerState
    {
        get { return Farmer.ValueRW.farmerState; }
        set { Farmer.ValueRW.farmerState = value; }
    }

    public float3 MoveTarget
    {
        get => Farmer.ValueRW.moveTarget;
        set => Farmer.ValueRW.moveTarget = value;
    }

    public float MoveSpeed
    {
        get => Farmer.ValueRW.moveSpeed;
        set => Farmer.ValueRW.moveSpeed = value;
    }
}
