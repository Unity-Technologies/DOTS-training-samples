using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public enum TargetTypes
{
    Enemy,
    Resource
}

[Serializable]
public struct C_Target : IComponentData
{
    public Entity Value;
}

public struct C_TargetType : ISharedComponentData
{
    public TargetTypes Type;
}