using System;
using Unity.Entities;
using Unity.Mathematics;


[Serializable]
public struct BarPoint : IComponentData {
    public float3 pos;
    public float3 oldPos;
    public bool anchor;
    public int neighborCount;
    public int index;
}
