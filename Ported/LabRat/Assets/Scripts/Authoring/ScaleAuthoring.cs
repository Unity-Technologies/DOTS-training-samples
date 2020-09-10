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
    public float Grow;
}

[GenerateAuthoringComponent]
public struct ScaleAuthoring : IComponentData
{
    public float Scale;

    [Header("Random Scale")]
    public bool Enable;
    public float MinScale;
    public float MaxScale;
}
