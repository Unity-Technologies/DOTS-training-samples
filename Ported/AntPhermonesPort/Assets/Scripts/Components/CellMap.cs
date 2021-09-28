using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public enum CellState : byte
{
	Empty,
	LineOfSight, // Straight line to food
	IsObstacle, // Contains wall or outer boundary
	IsFood
}

[GenerateAuthoringComponent]
public struct CellMap : IComponentData
{

}