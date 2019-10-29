using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct PartData : IComponentData
{
    public float radiusMult;
    public Color color;
    public Matrix4x4 matrix;
}
