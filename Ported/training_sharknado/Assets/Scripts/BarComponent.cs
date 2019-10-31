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
