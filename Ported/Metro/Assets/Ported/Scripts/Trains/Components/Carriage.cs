using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Carriage : IComponentData
{
    public int Index;
}