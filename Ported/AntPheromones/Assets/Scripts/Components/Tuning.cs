using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// Rename to GlobalVariables (or WorldSomething)
[GenerateAuthoringComponent]
public struct Tuning : IComponentData
{
	public float Speed;
	public float MaxLosDistSq;
	public float AntAngleRange;
	public float MinAngleWeight;
	public float PheromoneWeighting;
	public float AntFwdWeighting;
	public int PheromoneBuffer;
	public float PheromoneDecayStrength;
	public int Resolution;
	public float WorldSize;
	public float2 WorldOffset;

	public float DecayPeriod;
	public int DecayValue;

	public float PheromoneDropPeriod;
	public int PheromoneDropValue;

	public Entity AntFoodPrefab;
}
