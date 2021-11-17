using System;
using Unity.Entities;

[Serializable]
public struct TargetedBy : IComponentData
{
    public Entity Value;
}
