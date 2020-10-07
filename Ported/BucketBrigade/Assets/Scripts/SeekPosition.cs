using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SeekPosition : IComponentData
{
    public float3 TargetPos; // could use a float2 I guess, as we only need x + z coordinates.
    public float MaxVelocity;
}
