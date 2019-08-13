using System;
using Unity.Entities;

// Serializable attribute is for editor support.
// ReSharper disable once InconsistentNaming
[Serializable]
public struct CarComponent : IComponentData
{
    public float speed;
    public int currentSpline;
}
