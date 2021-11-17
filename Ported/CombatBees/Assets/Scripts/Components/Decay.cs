using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Decay : IComponentData
{
    public static readonly float Never = float.MaxValue;
    public static readonly float TotalTime = 10.0f;
    public float RemainingTime;
}
