using System;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Board entity
/// </summary>
[Serializable]
public struct LbBoard : IComponentData
{
    /// <summary>
    /// Board's size X
    /// </summary>
    public byte SizeX;

    /// <summary>
    /// Board's size Y
    /// </summary>
    public byte SizeY;
}
