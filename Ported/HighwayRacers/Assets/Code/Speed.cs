using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Speed : IComponentData
{
    [NonSerialized]
    public float Value;
}
