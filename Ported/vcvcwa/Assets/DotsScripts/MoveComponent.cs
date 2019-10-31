using System;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
[GenerateAuthoringComponent]
public struct MoveComponent : IComponentData
{
    public bool fly;
}