using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AnimationProperties : IComponentData
{
    public float ObjScaleFactor;
    public float ObjectScaleTime;
}
