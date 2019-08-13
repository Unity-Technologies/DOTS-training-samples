using System;
using Unity.Entities;

// Serializable attribute is for editor support.
// ReSharper disable once InconsistentNaming
[Serializable]
public struct InterpolatorTComponent : IComponentData
{
    public float t;
}
