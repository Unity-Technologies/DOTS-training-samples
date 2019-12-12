using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ArmComponent : IComponentData
{
    public float3 HandPosition;
    public float3 HandTarget;
    public float3 HandForward;
    public float3 HandUp;
    public float3 HandRight;

    public float ReachTimer;
    public float ThrowTimer;
}