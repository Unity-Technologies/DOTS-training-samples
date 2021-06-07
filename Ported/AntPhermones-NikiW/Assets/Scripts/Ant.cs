using System;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[NoAlias]
public struct Ant
{
    public float2 position;
    public float facingAngle;
    public ushort lifeTicks;

    public Ant(float2 pos)
    {
        position = pos;
        facingAngle = Random.value * math.PI * 2f;
        lifeTicks = 0;
    }
}