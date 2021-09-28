using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Config : IComponentData
{
    public int Speed = 1; // 1-9
	
	// Simulation Constants
	public const int AntCount = 100;

	// GreenDotDistance
	// SphereRadius
	// WallDistanceFromCenter
	// Total simulation size CellMapResolution 
}