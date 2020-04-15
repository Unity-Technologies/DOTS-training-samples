using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PercentComplete : IComponentData
{
    public float Value;
}
