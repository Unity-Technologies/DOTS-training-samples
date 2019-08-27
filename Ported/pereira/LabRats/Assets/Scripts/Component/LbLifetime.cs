using System;
using Unity.Entities;

/// <summary>
/// Lifetime value, when 0 the entity is marked for destruction
/// </summary>
[Serializable]
public struct LbLifetime : IComponentData
{
    /// <summary>
    /// Remaining lifetime
    /// </summary>
    public float Value;
}
