using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace AutoFarmers
{
    public static class Helper
    {
		public static void HarvestPlant(Farm farm,NativeArray<Entity> tiles,ComponentDataFromEntity<GroundTile> tileFromEntity ,ComponentDataFromEntity<Plant> plantFromEntity ,int2 position)
        {
			var tileEntity = tiles[PositionToHash(farm, position)];
			var tile = tileFromEntity[tileEntity];
			var plantEntity = tile.TilePlant;
			var plant = plantFromEntity[plantEntity];
			plant.harvested = true;
			tile.TilePlant = Entity.Null;
			tile.State = GroundState.Tilled;
			tileFromEntity[tileEntity] = tile;
			plantFromEntity[plantEntity] = plant;
        }

		public static void EaseToWorldPosition(ComponentDataFromEntity<Translation> translationFromEntity,Entity plantEntity, float3 position,float smooth)
        {
			var translation = translationFromEntity[plantEntity];
			translation.Value += (position - translation.Value) * smooth * 3f;
			translationFromEntity[plantEntity] = translation;
        }
		public static void SellPlant(Entity farmEntity,ComponentDataFromEntity<Money> moneyFromEntity,ComponentDataFromEntity<FarmerGlobalParams> farmerGlobalParamsFromEntity,ComponentDataFromEntity<DroneGlobalParams> droneGlobalParamsFromEntity, Entity plantEntity,ComponentDataFromEntity<Plant> plantFromEntity,int2 storePos,EntityCommandBuffer commandBuffer)
        {
			var plant = plantFromEntity[plantEntity];
			plant.Position = storePos;
			plantFromEntity[plantEntity] = plant;
			commandBuffer.AddComponent<Plant.Sold>(plantEntity);
			var money = moneyFromEntity[farmEntity];
			var farmerGlobalParams = farmerGlobalParamsFromEntity[farmEntity];
			var droneGlobalParams = droneGlobalParamsFromEntity[farmEntity];
			money.MoneyForFarmers++;
			money.MoneyForDrones++;
            if (money.MoneyForFarmers >= 10 & farmerGlobalParams.FarmerCount < farmerGlobalParams.MaxFarmerCount)
            {
				var farmer = commandBuffer.Instantiate(farmerGlobalParams.FarmerPrefab);
				var pos3D = new float3(storePos.x + 0.5f, 0.5f, storePos.y + 0.5f);
				commandBuffer.SetComponent(farmer, new Farmer() { Position = pos3D.xz });
				commandBuffer.SetComponent(farmer, new Translation() { Value = pos3D });
				money.MoneyForFarmers -= 10;
				farmerGlobalParams.FarmerCount++;
            }
			if(money.MoneyForDrones >= 50 & droneGlobalParams.DroneCount < droneGlobalParams.MaxDroneCount)
            {
				var random = Random.CreateFromIndex((uint)droneGlobalParams.DroneCount);
				var droneCount = math.min(5, droneGlobalParams.MaxDroneCount - droneGlobalParams.DroneCount);
				var pos = new float3(storePos.x + 0.5f, 0f, storePos.y + 0.5f);
				for (int i = 0; i < droneCount; i++)
                {
					var drone = commandBuffer.Instantiate(droneGlobalParams.DronePrefab);
					commandBuffer.SetComponent(drone, new Drone() { Position = pos, HoverHeight = random.NextFloat(2f, 3f), SearchTimer = random.NextFloat() });
					commandBuffer.SetComponent(drone, new Translation() { Value = pos });
				}
				droneGlobalParams.DroneCount += droneCount;
				money.MoneyForDrones -= 50;
            }

			moneyFromEntity[farmEntity] = money;
			farmerGlobalParamsFromEntity[farmEntity] = farmerGlobalParams;
			droneGlobalParamsFromEntity[farmEntity] = droneGlobalParams;

        }
		public static void SpawnPlant(EntityManager entityManager,Entity farmEntity ,int2 pos,int seed)
		{
			var farm = entityManager.GetComponentData<Farm>(farmEntity);
			var tiles = entityManager.GetBuffer<Farm.GroundTiles>(farmEntity).Reinterpret<Entity>();
			var plantPrefabs = entityManager.GetBuffer<PlantPrefabs>(farmEntity).Reinterpret<Entity>();
			var index = PositionToHash(farm, pos);
			var random = Random.CreateFromIndex((uint)index);
			var tileEntity = tiles[index];
			var plantEntity = entityManager.Instantiate(plantPrefabs[seed]);
			entityManager.SetComponentData(plantEntity,
				new Plant()
			{
				Position = pos,
				harvested = false,
				reserved = false,
			});
			entityManager.SetComponentData(plantEntity, new Translation() { Value = new float3(pos.x + 0.5f, 0, pos.y + 0.5f) });
			entityManager.SetComponentData(plantEntity, new Rotation() { Value = Quaternion.Euler(random.NextFloat(-5f, 5f), random.NextFloat(360f), random.NextFloat(-5f, 5f))});
			var tile = entityManager.GetComponentData<GroundTile>(tileEntity);
			tile.TilePlant = plantEntity;
			tile.State = GroundState.Plant;
			entityManager.SetComponentData(tileEntity, tile);
		}
		public static int PositionToHash(Farm farm, int2 pos) => pos.x + farm.MapSize.x * pos.y;
		public static void HashToPosition(Farm farm,int hash, out int x,out int y)
        {
			y = hash / farm.MapSize.x;
			x = hash - farm.MapSize.x * y;
			
        }
		public static void SpawnFarmer(EntityManager entityManager,Entity farmerPrefab,int2 pos)
        {
			var entity = entityManager.Instantiate(farmerPrefab);
			entityManager.SetComponentData(entity, new Farmer() { Position = pos });
        }
    }

}