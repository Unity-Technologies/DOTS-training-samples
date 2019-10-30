using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class SpawnerBase : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
	public GameObject Prefab;
	public int Count;

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		referencedPrefabs.Add(Prefab);
	}

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		var spawnerData = new ComponentBase
		{
			Prefab = conversionSystem.GetPrimaryEntity(Prefab),
			Value = CountX
		};
		dstManager.AddComponentData(entity, spawnerData);
	}
}