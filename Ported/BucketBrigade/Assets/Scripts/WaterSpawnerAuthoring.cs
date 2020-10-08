using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct WaterSpawnerElement : IBufferElementData
{
	public Entity Instance;
	public Side Side;
};

public struct WaterSpawner : IComponentData
{
	public DynamicBuffer<Entity> Instances2;
	//public Entity[] Instances;
	public Side[] Sides;
	public float3[] Offsets;
	public float[] RotationYs;
}

public enum Side
{
	North,
	East,
	South,
	West
}

[System.Serializable]
public struct WaterInstance
{
	public GameObject WaterPrefab;

	public Side Side;

	public float3 Offset;

	public float RotationY;
}

public class WaterSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
	public WaterInstance[] Instances;

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
  		referencedPrefabs.AddRange(Instances.Select(inst => inst.WaterPrefab));
	}

	public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
	{
		Entity[] instanceArray = Instances.Select(inst => conversionSystem.GetPrimaryEntity(inst.WaterPrefab)).ToArray();
		NativeArray<Entity> instanceNativeArray = new NativeArray<Entity>();
		instanceNativeArray.CopyFrom(instanceArray);
		DynamicBuffer<Entity> instanceDynamicBuffer = new DynamicBuffer<Entity>();
		instanceDynamicBuffer.AddRange(instanceNativeArray);
		Side[] sides = Instances.Select(inst => inst.Side).ToArray();
		float3[] offsets = Instances.Select(inst => inst.Offset).ToArray();
		float[] rotationYs = Instances.Select(inst => inst.RotationY).ToArray();

		entityManager.AddBuffer<WaterSpawnerElement>(entity);
		entityManager.AddComponentData(entity, new WaterSpawner
		{
			Instances2 = instanceDynamicBuffer,
			Sides = sides,
			Offsets = offsets,
			RotationYs = rotationYs,
		});
	}
}
