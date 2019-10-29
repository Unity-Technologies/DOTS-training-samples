using Unity.Entities;
using System;

/// <summary>
/// Component that store's the player ID
/// </summary>
[Serializable]
public struct LbPlayer : IComponentData
{
    public byte Value;
}

