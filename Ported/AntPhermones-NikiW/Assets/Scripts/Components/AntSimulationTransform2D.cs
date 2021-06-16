using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[NoAlias]
public struct AntSimulationTransform2D : IComponentData
{
    public float2 position;
    public float facingAngle; // NWALKER: Move to its own struct.
}