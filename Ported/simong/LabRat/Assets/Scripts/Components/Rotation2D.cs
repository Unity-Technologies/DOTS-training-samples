using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Position2D : IComponentData
{
    public float2 Value;
}