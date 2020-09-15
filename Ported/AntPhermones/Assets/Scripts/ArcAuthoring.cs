using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Arc : IComponentData
{
    public const float Width = 1.0f;
    
    public float Radius;
    public float StartAngle;
    public float EndAngle;
}
