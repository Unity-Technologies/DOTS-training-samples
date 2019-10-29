using Unity.Entities;
using System;

/// <summary>
/// Contains all data needed by the cursor to spawn an arrow
/// </summary>
[Serializable]
public struct LbArrowPrefab : IComponentData
{
    public Entity Value;
}