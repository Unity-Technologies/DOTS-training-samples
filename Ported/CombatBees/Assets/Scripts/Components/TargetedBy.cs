using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargetedBy : IComponentData
{
    public Entity Value;
}
