using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct LocalTranslation : IComponentData
{
    public float2 Value;
}
