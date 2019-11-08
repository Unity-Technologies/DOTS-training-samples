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

	/*Coroutine coro;
    bool InAlternate = false;

	void OnEnable() {
		Counter = Frequency;
	}

    void OnDisable() {
        if (coro != null) {
            StopCoroutine(coro);
            coro = null;
        }
    }

    void Start() {
        if (AlternatePrefab)
            coro = StartCoroutine(Alternates());
    }

    IEnumerator Alternates() {
        while (enabled) {
            yield return new WaitForSeconds(Random.Range(4.0f, 15f));
            InAlternate = true;
            yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
            Spawn(AlternatePrefab);
            yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
            InAlternate = false;
        }
    }*/
	
	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		referencedPrefabs.Add(Prefab);
		referencedPrefabs.Add(AlternatePrefab);
	}

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.SetName(entity, "Spawner");
		dstManager.AddComponentData(entity, new SpawnerComponent
		{
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
