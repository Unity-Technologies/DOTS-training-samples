using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Arrow : IComponentData
{
    public int Position;
}
