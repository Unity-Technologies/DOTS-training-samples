using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct Config : IComponentData
{
    public int Speed; // Will become dynamic

	// Simulation Constants
	public int AntCount;

	public float MoveSpeed;
	public float RotationSpeed; // degrees per second

	public float RotationAngle;
	public int RingCount;
	public int RingDistance;
	public int AngleSize;

	public int MaxEntriesPerCircle;
}
