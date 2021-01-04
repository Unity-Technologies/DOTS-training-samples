using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Animation.Hybrid;
namespace AutoFarmers
{
	[DisallowMultipleComponent]
	internal class FarmAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
	{
		public int2 MapSize;

		public int StoreCount;

		public int RockSpawnAttempts;

		public int SeedOffset;

		public GameObject PlantPrefab;

		public GameObject GroundTilePrefab;
		
		public GameObject RockPrefab;

		public GameObject StorePrefab;

		public AnimationCurve SoldPlantYCurve;
		public AnimationCurve SoldPlantXZScaleCurve;
		public AnimationCurve SoldPlantYScaleCurve;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
            var plantPrefab = conversionSystem.GetPrimaryEntity(PlantPrefab);
			dstManager.AddComponent<Plant>(plantPrefab);
			dstManager.AddComponent<Plant.Growth>(plantPrefab);
			dstManager.AddComponent<NonUniformScale>(plantPrefab);
			var groundTilePrefab = conversionSystem.GetPrimaryEntity(GroundTilePrefab);
			dstManager.AddComponent<GroundTile>(groundTilePrefab);
			dstManager.AddComponent<GroundTile.Tilled>(groundTilePrefab);
			var rockPrefab = conversionSystem.GetPrimaryEntity(RockPrefab);
			dstManager.AddComponent<Rock>(rockPrefab);
			dstManager.AddComponentData(rockPrefab, new NonUniformScale() { Value = 1 });
			var storePrefab = conversionSystem.GetPrimaryEntity(StorePrefab);
            dstManager.AddComponentData(entity, new Farm()
			{
				MapSize = MapSize,
				SeedOffset = SeedOffset,
				PlantPrefab = plantPrefab,
				GroundTilePrefab = groundTilePrefab,
				RockPrefab = rockPrefab,
				StorePrefab = storePrefab,
				SoldPlantYCurve = SoldPlantYCurve.ToDotsAnimationCurve(),
				SoldPlantXZScaleCurve = SoldPlantXZScaleCurve.ToDotsAnimationCurve(),
				SoldPlantYScaleCurve = SoldPlantYScaleCurve.ToDotsAnimationCurve(),
			});
			dstManager.AddComponentData(entity, new Farm.StoreCount() { Value = StoreCount });
			dstManager.AddComponentData(entity, new Farm.RockSpawnAttempts() { Value = RockSpawnAttempts });
			dstManager.AddComponentData(entity, new Money() {MoneyForFarmers = 5, MoneyForDrones = 0,});
		}

		public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
		{
			referencedPrefabs.Add(PlantPrefab);
			referencedPrefabs.Add(GroundTilePrefab);
			referencedPrefabs.Add(RockPrefab);
			referencedPrefabs.Add(StorePrefab);
		}
	}
}
