using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Config : IComponentData
{
    public float3 bounds;
    public float3 gravity;
    public int beeCount;
    public int foodCount;
    public float2 foodBounds;
    public float bloodDecay;
}
