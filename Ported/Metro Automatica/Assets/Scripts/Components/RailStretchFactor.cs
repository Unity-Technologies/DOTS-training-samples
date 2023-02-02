using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEditor;
using UnityEngine;


[MaterialProperty("_StretchFactor", -1)]
public struct RailStretchFactor : IComponentData
{
    public float2 StretchFactor;
}
