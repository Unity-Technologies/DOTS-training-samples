using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Config
{
    public static readonly int beeCount = 10;
    public static readonly int foodCount = 10;
    public static readonly float3 bounds = new float3(10, 10, 10);
    public static readonly float bloodDecay = 1.0f;
    public static readonly float3 gravity = new float3(0, -20, 0);
}
