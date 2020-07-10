using Unity.Entities;
using Unity.Mathematics;

// Fire simulation grid settings
[GenerateAuthoringComponent]
public struct FireGridSpawner : IComponentData
{
	// Prefab for the fire cells
    public Entity FirePrefab;
    // Number of fire points at start
    public int StartingFireCount;
    // Prefab for the wanter puddles
    public Entity WaterPrefab;
    // Distance from the grid to the river
    public float DistanceToGrid;
    // Number of water cells per side
    public int ElementsPerSide;
}