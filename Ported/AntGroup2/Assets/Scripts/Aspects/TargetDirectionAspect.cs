using System.Collections;
using System.Collections.Generic;using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

readonly partial struct TargetDirectionAspect : IAspect
{
    readonly RefRW<TargetDirection> TargetDirection;
    
    public float Direction
    {
        get => TargetDirection.ValueRO.Angle;
        set => TargetDirection.ValueRW.Angle = value;
    }
}
