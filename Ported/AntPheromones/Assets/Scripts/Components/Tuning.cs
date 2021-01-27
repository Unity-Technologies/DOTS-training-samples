using Unity.Entities;
using Unity.Mathematics;

// Rename to GlobalVariables (or WorldSomething)
[GenerateAuthoringComponent]
public struct Tuning : IComponentData
{
	public float Speed;
	public float AntAngleRange;
	public float AntFwdWeighting;
	public int PheromoneBuffer;
	public float PheromoneDecayStrength;
	public int Resolution;
	public float WorldSize;
	public float2 WorldOffset;
	public float AntPheromoneStrength;
	public Entity AntFoodPrefab;
}
