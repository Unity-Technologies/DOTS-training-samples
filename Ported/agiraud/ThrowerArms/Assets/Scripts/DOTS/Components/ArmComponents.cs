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

public struct UpAxis : IComponentData
{
    public float3 Value;
}

public struct ArmTarget : IComponentData
{
    public float3 Value;
}
