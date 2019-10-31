using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ScaledRenderer : IComponentData
{
    public int Max;
    public float XZScaleAtZero;
    public float XZScaleAtMax;
    public float YScaleAtZero;
    public float YScaleAtMax;
}
