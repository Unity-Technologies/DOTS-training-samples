using Unity.Entities;
using UnityMath = Unity.Mathematics;


public struct AntMovementState : IComponentData
{
    public UnityMath.float2 origin;
    public UnityMath.float2 delta;
    public UnityMath.float2 result => origin + delta;

    // canary value is origin x
    public readonly bool IsMovement => (delta == UnityMath.float2.zero).x;
}