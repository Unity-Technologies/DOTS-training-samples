using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Smoothing : IComponentData
{
    public float3 SmoothPosition;
    public Vector3 SmoothDirection;
}