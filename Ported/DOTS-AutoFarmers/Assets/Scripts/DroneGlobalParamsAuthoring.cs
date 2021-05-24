using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
namespace AutoFarmers
{
    [DisallowMultipleComponent]
    internal class DroneGlobalParamsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
		public GameObject DronePrefab;
		public int MaxDroneCount;
		[Range(0f, 1f)]
		public float MoveSmooth;
		[Range(0f, 1f)]
		public float CarrySmooth;
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var dronePrefab = conversionSystem.GetPrimaryEntity(DronePrefab);
			dstManager.AddComponent<Drone>(dronePrefab);
			dstManager.AddComponentData(entity, new DroneGlobalParams()
			{
				DronePrefab = conversionSystem.GetPrimaryEntity(DronePrefab),
				MaxDroneCount = MaxDroneCount,
				MoveSmooth = MoveSmooth,
				CarrySmooth = CarrySmooth,
			});
		}

		public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
		{
			referencedPrefabs.Add(DronePrefab);
		}
	}
}

