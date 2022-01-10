using Unity.Entities;
using Unity.Mathematics;

public enum Intention 
{
	None,
	SmashRocks,
	TillGround,
	PlantSeeds,
	SellPlants
}

public struct Farmer : IComponentData
{
	public Intention Intention;
	public float4 TillableZone;
}
