using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct ThrowAtState : IComponentData
{
    public float ThrowTimer;
    public float3 StartPosition;
    public float3 AimVector;
    public Entity AimedTargetEntity;
    public Entity HeldEntity;
}
