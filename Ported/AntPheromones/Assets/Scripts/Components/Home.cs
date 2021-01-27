using System;

using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Home : IComponentData
{
    public Color color;
    public float radius;
}
