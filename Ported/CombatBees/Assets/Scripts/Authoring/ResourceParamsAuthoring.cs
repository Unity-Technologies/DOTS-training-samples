using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using System;
using Unity.Collections;

/*
[GenerateAuthoringComponent]
public struct ResourceParamsAuthoring : IComponentData
{
	public float resourceSize;
	public float snapStiffness;
	public float carryStiffness;
	public int maxResCount;
}
*/


public class ResourceParamsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	// authoring fields go here
	public float resourceSize;
	public float snapStiffness;
	public float carryStiffness;
	public int beesPerResource;

	//public GameObject field;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
		ResourceParams resParams = new ResourceParams
		{
			resourceSize = this.resourceSize,
			snapStiffness = this.snapStiffness,
			carryStiffness = this.carryStiffness,
			beesPerResource = this.beesPerResource
		};

		dstManager.AddComponentData<ResourceParams>(entity, resParams);

		/*
		int gridCountsX = (int)field.GetComponent<FieldAuthoring>().size.x / (int)this.resourceSize;
		int gridCountsZ = (int)field.GetComponent<FieldAuthoring>().size.z / (int)this.resourceSize;
		int2 gridCounts = new int2(gridCountsX, gridCountsZ);

		float gridSizeX = field.GetComponent<FieldAuthoring>().size.x / gridCounts.x;
		float gridSizeZ = field.GetComponent<FieldAuthoring>().size.z / gridCounts.y;
		float2 gridSize = new float2(gridSizeX, gridSizeZ);
		*/

		int gridCountsX = (int)(100f / this.resourceSize);
		int gridCountsZ = (int)(30f / this.resourceSize);
		int2 gridCounts = new int2(gridCountsX, gridCountsZ);

		float gridSizeX = 100f / gridCounts.x;
		float gridSizeZ = 30f / gridCounts.y;
		float2 gridSize = new float2(gridSizeX, gridSizeZ);

		float minGridPosX = (gridCounts.x - 1f) * -.5f * gridSize.x;
		float minGridPosZ = (gridCounts.y - 1f) * -.5f * gridSize.y;
		float2 minGridPos = new float2(minGridPosX, minGridPosZ);

		ResourceGridParams resGridParams = new ResourceGridParams
		{
			gridCounts = gridCounts,
			gridSize = gridSize,
			minGridPos = minGridPos
			//stackHeights = new NativeArray<int>(gridCounts.x * gridCounts.y, Allocator.Persistent)
		};

		dstManager.AddComponentData<ResourceGridParams>(entity, resGridParams);

		var stackHeights = dstManager.AddBuffer<StackHeightParams>(entity);
		stackHeights.EnsureCapacity(gridCounts.x * gridCounts.y);
		for (int i = 0; i < gridCounts.x; i++)
        {
			for(int j = 0; j < gridCounts.y; j++)
            {
				stackHeights.Add(new StackHeightParams { Value = 0 });
			}
        }
	}
}

public struct ResourceParams : IComponentData
{
	public float resourceSize;
	public float snapStiffness;
	public float carryStiffness;
	//public float spawnRate;
	//float spawnTimer = 0f;
	public int beesPerResource;
	//public int startResourceCount;
}

public struct ResourceGridParams : IComponentData
{
	public int2 gridCounts;
	public float2 gridSize;
	public float2 minGridPos;
	//public NativeArray<int> stackHeights;
}

public struct StackHeightParams : IBufferElementData
{
	public int Value;
}
