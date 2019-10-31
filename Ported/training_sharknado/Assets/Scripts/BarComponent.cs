using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct Bar : IComponentData
{
    public float3 pos1;
    public float3 pos2;
    public float3 oldPos1;
    public float3 oldPos2;
    public float length;
    public int neighbors1;
    public int neighbors2;
}

public struct BarPoint1 : IComponentData
{
    public float3 pos;
    public float3 oldPos;
    public int neighbors;
}
public struct BarPoint2 : IComponentData
{
    public float3 pos;
    public float3 oldPos;
    public int neighbors;
}
public struct BarAveragedPoints1 : IComponentData
{
    public float3 pos;
}
public struct BarAveragedPoints2 : IComponentData
{
    public float3 pos;
}
public struct BarLength : IComponentData
{
    public float value;
}
