using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct Config : IComponentData
{
    public int Speed;

	public float WorldSize; // e.g. Lower left of bounding box is at -WorldSize/2, -WorldSize/2

	public int CellMapResolution;
	public bool DisplayCellMap;

	// Simulation Constants
	public int AntCount;

	public float MoveSpeed;
	public float RotationSpeed; // degrees per second
	public float AntAcceleration;
	public float AntHasFoodPeromoneMultiplier; // Multiplier for pheromones once an ant has food

	public int RingCount;
	public int RingDistance;
	public int RingAngleSize;
	public int MaxEntriesPerRing;

	public float PheromoneProductionPerSecond; // per second pheromone dropped by ant

	public float PheromoneDecayRate; // 0 is instant decay, 1 is no decay at all

	public uint RandomSeed;

	public float RandomSteering;
	public float PheromoneSteerStrength;
	public float WallSteerStrength;
	public float GoalSteerStrength;

}
