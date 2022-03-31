using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AdhocResourceManager : MonoBehaviour
{
	public GameObject ResourcePrefab;
	public static AdhocResourceManager instance;

	void Awake()
    {
		if (instance == null)
			instance = this;
		else
			Destroy(this);
    }
	public void Spawn(float3 spawnerPosition)
	{
// TODO REMOVE
		//ResourcePrefab.transform.position = spawnerPosition;
		//var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		//var spawner = entityManager.CreateEntityQuery(typeof(SpawnerData)).ToComponentDataArray<SpawnerData>(Unity.Collections.Allocator.Temp);
		//entityManager.Instantiate(spawner.First().Entity);
	}
}
