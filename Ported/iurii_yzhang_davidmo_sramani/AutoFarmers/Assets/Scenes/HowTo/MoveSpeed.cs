using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct MoveSpeed : IComponentData
{
    public float3 Value;
}

