using Unity.Entities;
using UnityMath = Unity.Mathematics;


public struct AntMovementState : IComponentData
{
    public UnityMath.float2 origin;
    public UnityMath.float2 delta;
    public UnityMath.float2 result => origin + delta;

    // canary value is origin x
    public readonly bool IsValid => float.IsNaN(origin.x);

    public AntMovementState(in float originX = float.NaN, in float originY = float.NaN, in float deltaX = float.NaN, in float deltaY = float.NaN)
    {
        origin.x = originX;
        origin.y = originY;
            
        delta.x = deltaX;
        delta.y = deltaY;
    }

    public void Reset() => origin.x = float.NaN;
}