using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PercentComplete : IComponentData
{
    [NonSerialized]
    public float Value;
}
