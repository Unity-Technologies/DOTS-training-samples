using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct ResourceParamsAuthoring : IComponentData
{
	public float resourceSize;
	public float snapStiffness;
	public float carryStiffness;
	public int maxResCount;
}

/*
public class ResourceParamsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	// authoring fields go here
	public float resourceSize;
	public float snapStiffness;
	public float carryStiffness;
	
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
		ResourceParams resParams = new ResourceParams
		{
			resourceSize = this.resourceSize,
			snapStiffness = this.snapStiffness,
			carryStiffness = this.carryStiffness
		};

		dstManager.AddComponentData(entity, resParams);
    }
}

public struct ResourceParams : IComponentData
{
	public float resourceSize;
	public float snapStiffness;
	public float carryStiffness;
	//public float spawnRate;
	//float spawnTimer = 0f;
	//public int beesPerResource;
	//public int startResourceCount;
}

public struct ResourceGridParams : IComponentData
{
	int2 gridCounts;
	float2 gridSize;
	float2 minGridPos;
	//int[,] stackHeights;
}
*/
