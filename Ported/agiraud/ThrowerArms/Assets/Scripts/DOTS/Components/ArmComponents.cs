using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct ArmSpawnerData : IComponentData
{
    public int Count;
    public Entity BoneEntityPrefab;
}

public struct BoneJoint : IBufferElementData
{
    public float3 JointPos;
}

[Serializable]
[WriteGroup(typeof(LocalToWorld))]
public struct BoneData : IComponentData
{
    public Entity Parent;
    public int ChainIndex;
    public float Thickness;
}

public struct HandAxis : IComponentData
{
    public float3 Up;
    public float3 Forward;
    public float3 Right;
}

public struct ArmTarget : IComponentData
{
    public float3 Value; // Hand target
    // (x, y, z) pos and w size
    public float4 LastRockPosSize;
    public bool IsHolding;

    public Entity TargetRock;
    public Entity TargetCan;

    public float3 AimVector;
    public float3 WindupHandTarget;
}

public struct Timers : IComponentData
{
    public float TimeOffset;
    
    public float Reach;
    public float Windup;
    public float Throw;
    public float GrabT;
}