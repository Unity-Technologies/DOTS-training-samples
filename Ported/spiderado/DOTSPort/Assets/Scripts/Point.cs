using System;
using Unity.Entities;
using Unity.Mathematics;


[Serializable]
public struct BarPoint : IComponentData {
    public float3 pos;
    public float3 oldPos;
    public int anchor;
    public int neighborCount;
    public int index;
}
