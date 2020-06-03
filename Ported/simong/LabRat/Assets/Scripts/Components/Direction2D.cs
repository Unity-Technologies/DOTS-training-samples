using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Flags]
public enum GridDirection
{
    NORTH = (1 << 0),
    EAST = (1 << 1),
    SOUTH = (1 << 2),
    WEST = (1 << 3)
}

[GenerateAuthoringComponent]
public struct Direction2D : IComponentData
{
    public GridDirection Value;
}

