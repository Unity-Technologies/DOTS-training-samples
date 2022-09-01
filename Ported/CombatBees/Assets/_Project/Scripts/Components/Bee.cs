using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

[Serializable]
public struct Bee : IComponentData
{
    public float TeamAttraction;
    public float TeamRepulsion;
    public float Damping;
    public float FlightJitter;
    public float RotationSharpness;
    public float Aggression;
    public float ChaseForce;
    public float MinBeeSize;
    public float MaxBeeSize;
    public float SpeedStretch;
    public float CarryForce;
    public float GrabDistance;
    public float AttackDistance;
    public float AttackForce;
    public float HitDistance;
    public float MaxSpawnSpeed;
    
    [NonSerialized]
    public float3 Velocity;
    [NonSerialized]
    public Unity.Mathematics.Random Random;
}

[Serializable]
public struct BeeTargetEnemy : IComponentData
{
    public Entity Target;
}

[Serializable]
public struct BeeTargetResource : IComponentData
{
    public Entity Target;
}

[Serializable]
public struct BeeCarryingResource : IComponentData
{
}

[Serializable]
public struct BeeState : ISystemStateComponentData
{
    public Team Team;
}
