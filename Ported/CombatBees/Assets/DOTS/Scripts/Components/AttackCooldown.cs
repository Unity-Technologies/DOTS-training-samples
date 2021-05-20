using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct AttackCooldown : IComponentData
{
    public float Value;
}
