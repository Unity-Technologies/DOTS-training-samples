using System;
using Unity.Entities;

/// <summary>
/// Speed value for Entities that can rotate
/// </summary>
[Serializable]
public struct LbRotationSpeed : IComponentData
{
    public float Value;
}
