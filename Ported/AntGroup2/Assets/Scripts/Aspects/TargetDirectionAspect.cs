using System.Collections;
using System.Collections.Generic;using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

readonly partial struct TargetDirectionAspect : IAspect
{
    readonly RefRW<TargetDirection> TargetDirection;
    
    public float2 Direction
    {
        get => TargetDirection.ValueRO.Direction;
        set => TargetDirection.ValueRW.Direction = value;
    }
}
