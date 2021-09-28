using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



public struct Config : IComponentData
{
    public const int Speed =1; // Will become dynamic

	// Simulation Constants
	public const int AntCount = 100;

	public const float MoveSpeed = 5f;
	public const float RotationSpeed = 360f; // degrees per second

	public const float RotationAngle = 360f;

	public const int RingCount = 3;
	// GreenDotDistance
	// SphereRadius
	// WallDistanceFromCenter
	// Total simulation size CellMapResolution 

}
