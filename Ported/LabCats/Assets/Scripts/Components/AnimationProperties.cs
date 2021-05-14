using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct BounceScaleAnimationProperties : IComponentData
{
    public float TargetScale;
    public float OriginalScale;
    public float AnimationDuration;
    public float AccumulatedTime;
}

public struct RotateAnimationProperties : IComponentData
{
    public quaternion TargetRotation;
    public quaternion OriginalRotation;
    public float AnimationDuration;
    public float AccumulatedTime;
}
