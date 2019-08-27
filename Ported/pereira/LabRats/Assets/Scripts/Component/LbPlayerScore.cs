using System;
using Unity.Entities;

/// <summary>
/// Player score data
/// </summary>
[Serializable]
public struct LbPlayerScore : IComponentData
{
    /// <summary>
    /// Score value
    /// </summary>
    public int Value;
}
