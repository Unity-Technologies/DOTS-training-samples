using System;
using Unity.Entities;

[Serializable]
public struct AttackTimer : IComponentData
{
    public float Value;
}
