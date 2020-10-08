using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Cell : IComponentData
{
    public int Index;
}
