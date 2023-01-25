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

    public Entity HeldEntity
    {
        get => Farmer.ValueRW.heldEntity;
    }

    public bool HoldingEntity
    {
        get => Farmer.ValueRW.holdingEntity;
    }

    public bool HasTarget
    {
        get => Farmer.ValueRW.holdingEntity;
    }

    public Entity CurrentTarget
    {
        get => Farmer.ValueRW.currentlyTargeted;
    }

    public void AttachEntity(Entity e)
    {
        Farmer.ValueRW.heldEntity = e;
        Farmer.ValueRW.holdingEntity = true;
    }

    public void DetachEntity()
    {
        Farmer.ValueRW.holdingEntity = false;
    }

    public void TargetEntity(Entity e)
    {
        Farmer.ValueRW.currentlyTargeted = e;
        Farmer.ValueRW.hasTarget = true;
    }

    public void ReleaseTarget()
    {
        Farmer.ValueRW.hasTarget = false;
    }
}
