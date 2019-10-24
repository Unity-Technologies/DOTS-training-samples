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
    public float3 Value;
}
