using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Config : IComponentData
{
    public const int Speed =1; // Will become dynamic

	// Simulation Constants
	public const int AntCount = 100;

	// GreenDotDistance
	// SphereRadius
	// WallDistanceFromCenter
	// Total simulation size CellMapResolution 

}
