using System;
using Unity.Entities;

[Serializable]
public struct AttackCooldown : IComponentData
{
    public float Value;
}
