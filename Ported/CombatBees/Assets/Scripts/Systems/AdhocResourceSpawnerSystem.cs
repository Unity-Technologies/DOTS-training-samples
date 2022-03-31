
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class AdhocResourceSpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var prefabSet = GetSingleton<PrefabSet>();

		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			var instance = EntityManager.Instantiate(prefabSet.ResourcePrefab);
			float3 pos = new float3(0f, 0f, 0f);
			var translation = new Translation { Value = pos };
			EntityManager.SetComponentData(instance, translation);
		}




	}
}