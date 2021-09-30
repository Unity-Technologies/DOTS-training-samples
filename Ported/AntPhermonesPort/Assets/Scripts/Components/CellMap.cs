using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

public enum CellState : byte
{
	Empty,
    IsNest,
    IsFood,
    IsObstacle,                 // Contains wall or outer boundary
    HasLineOfSightToFood,       // Has direct line of sight with food
    HasLineOfSightToNest,       // Has direct line of sight with nest
    HasLineOfSightToBoth        // Has direct line of sight with both food and nest
}

[GenerateAuthoringComponent]
public struct CellMap : IBufferElementData
{
	public CellState state;
}