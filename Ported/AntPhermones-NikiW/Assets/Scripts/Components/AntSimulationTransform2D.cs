using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

// NWALKER: Change this to a local to world system.
[NoAlias]
public struct AntSimulationTransform2D : IComponentData
{
    [GhostField(Quantization = 100, Smoothing = SmoothingAction.InterpolateAndExtrapolate, MaxSmoothingDistance = 10)]
    public float2 position;
    [GhostField(Quantization = 180, Smoothing = SmoothingAction.InterpolateAndExtrapolate)]
    public float facingAngle;
}