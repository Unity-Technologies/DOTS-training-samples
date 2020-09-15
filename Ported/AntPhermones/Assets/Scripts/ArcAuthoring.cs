using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class Arc : IComponentData
{
    public float Radius;
    public float StartAngle;
    public float EndAngle;
}
