using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECSExamples {

public class Spawner : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs {
	public float Max = Mathf.Infinity;
	public float Frequency = 0.2f;
	float Counter = 0f;
	int TotalSpawned = 0;

	public GameObject Prefab;
    public GameObject AlternatePrefab;

	public Transform targetParent;
	public Board board;

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		referencedPrefabs.Add(Prefab);
		referencedPrefabs.Add(AlternatePrefab);
	}

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
		dstManager.SetName(entity, Prefab.name);
#endif
		var spawnerType = SpawnerType.Eaten;
		if (Prefab.name.Contains("Cat"))
			spawnerType = SpawnerType.Eater;
		dstManager.AddComponentData(entity, new SpawnerComponent
		{
			PrimaryType = spawnerType,
			Prefab = conversionSystem.GetPrimaryEntity(Prefab),
			AlternatePrefab = conversionSystem.GetPrimaryEntity(AlternatePrefab),
			Counter = Counter,
			Frequency = Frequency,
			Max = Max,
			TotalSpawned = TotalSpawned,
			InAlternate = false,
			Timer = 0f
		});
	}
}

}
