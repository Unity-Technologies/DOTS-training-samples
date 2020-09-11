using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[WriteGroup(typeof(LocalToWorld))]
public struct Size : IComponentData
{
    public float Value;
}

public struct SizeGrown : IComponentData
{
    public float Grow;
}
