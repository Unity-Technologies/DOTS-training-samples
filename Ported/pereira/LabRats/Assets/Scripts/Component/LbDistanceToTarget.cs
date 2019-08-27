using System;
using Unity.Entities;

/// <summary>
/// Distance to reach the next cell value, when 0 the entity reach their destination
/// </summary>
[Serializable]
public struct LbDistanceToTarget : IComponentData
{
    /// <summary>
    /// Remaining distance to the target cell
    /// </summary>
    public float Value;
}
