using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct CurrentTarget : IComponentData
{
    public float2 Value;
}
