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
    WEST = (1 << 3),
    ALL = NORTH | EAST | SOUTH | WEST
}

[GenerateAuthoringComponent]
public struct Direction2D : IComponentData
{
    public GridDirection Value;

    public static bool DirectionsAreOpposite(GridDirection first, GridDirection second)
    {
        return (first == GridDirection.NORTH && second == GridDirection.SOUTH)
            || (first == GridDirection.SOUTH && second == GridDirection.NORTH)
            || (first == GridDirection.EAST && second == GridDirection.WEST)
            || (first == GridDirection.WEST && second == GridDirection.EAST);
    }
}

