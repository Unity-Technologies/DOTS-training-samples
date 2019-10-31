using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Point : IComponentData {
    public float3 pos;
    // Used for ParticleManager originally - maybe move to child object?
}

[Serializable]
public struct BarPoint : IComponentData {
    public float3 pos;
    public float3 oldPos;
    public bool anchor;
    public int neighborCount;
    public int index;
}
