using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace AutoFarmers
{
	[DisallowMultipleComponent]
	internal class FarmerGlobalParamsAuthoring : MonoBehaviour, IConvertGameObjectToEntity,IDeclareReferencedPrefabs
	{
		public GameObject FarmerPrefab;
	

		public int InitialFarmerCount;
		public int MaxFarmerCount;
		[Range(0f, 1f)]
		public float MovementSmooth;
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
            var farmerPrefab = conversionSystem.GetPrimaryEntity(FarmerPrefab);
			dstManager.AddComponent<Farmer>(farmerPrefab);
			dstManager.AddBuffer<Path>(farmerPrefab);
            dstManager.AddComponentData(entity, new FarmerGlobalParams() 
			{
				FarmerPrefab = farmerPrefab,
				MaxFarmerCount = MaxFarmerCount,
				MovementSmooth = MovementSmooth,
			});
			dstManager.AddComponentData(entity, new SpawnFarmer() { Count = InitialFarmerCount });
		}

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
			referencedPrefabs.Add(FarmerPrefab);
        }
    }
}
