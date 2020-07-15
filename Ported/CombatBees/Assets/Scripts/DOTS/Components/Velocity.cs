using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct Velocity : IComponentData
{
    public Vector3 Value;
}
