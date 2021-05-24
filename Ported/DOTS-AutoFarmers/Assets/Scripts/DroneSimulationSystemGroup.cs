using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class DroneSimulationSystemGroup : ComponentSystemGroup
    {
       
    }
	[UpdateInGroup(typeof(DroneSimulationSystemGroup))]
    public class SearchPlantSystem : SystemBase
	{
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
        {
			var farm = GetSingleton<Farm>();
			var tiles = GetBuffer<Farm.GroundTiles>(GetSingletonEntity<Farm.GroundTiles>()).AsNativeArray().Reinterpret<Entity>();
			var deltaTime = Time.DeltaTime;
			var parallelCommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
			Entities
				.WithReadOnly(tiles)
				.WithNone<Drone.TargetPlant,Drone.SellPlant>()
				.ForEach((Entity entity,int entityInQueryIndex,ref Drone drone, in Translation translation) =>
			{
				ref readonly var smoothPosition = ref translation.Value;
				var targetPos = new float3(drone.Position.x, drone.HoverHeight, drone.Position.z);
				ref var position = ref drone.Position;
				ref var searchTimer = ref drone.SearchTimer;
				{
					if (searchTimer < 0f)
					{
						var pathing = new Pathing(farm, tiles, GetComponentDataFromEntity<GroundTile>(true));

						int plantTileHash = pathing.SearchForOne((int2)smoothPosition.xz, 30, new Pathing.IsNavigableAll(), new Farm.IsHarvestableAndUnreserved(GetComponentDataFromEntity<Plant>(true), GetComponentDataFromEntity<Plant.Growth>(true)), pathing.FullMapZone);
						if (plantTileHash != -1)
						{
							var tile = GetComponent<GroundTile>(tiles[plantTileHash]);
							var targetPlantEntity = tile.TilePlant;
							var targetPlant = GetComponent<Plant>(targetPlantEntity);
							targetPlant.reserved = true;
							SetComponent(targetPlantEntity, targetPlant);
							parallelCommandBuffer.AddComponent(entityInQueryIndex, entity, new Drone.TargetPlant() { Value = targetPlantEntity });
						}
						searchTimer = 1f;
						pathing.Dispose();
					}
					else
					{
						searchTimer -= deltaTime;
					}
				}
				position.x = Mathf.MoveTowards(position.x, targetPos.x, Drone.xzSpeed * deltaTime);
				position.y = Mathf.MoveTowards(position.y, targetPos.y, deltaTime * Drone.ySpeed);
				position.z = Mathf.MoveTowards(position.z, targetPos.z, Drone.xzSpeed * deltaTime);

			}).Schedule();
			commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
	[UpdateInGroup(typeof(DroneSimulationSystemGroup))]
	public class HarvestPlantSystem : SystemBase
	{
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
		{
			var farm = GetSingleton<Farm>();
			var tiles = GetBuffer<Farm.GroundTiles>(GetSingletonEntity<Farm.GroundTiles>()).AsNativeArray().Reinterpret<Entity>();
			var deltaTime = Time.DeltaTime;
			var parallelCommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
			Entities
				.WithReadOnly(tiles)
				.WithNone<Drone.SellPlant>()
				.ForEach((Entity entity,int entityInQueryIndex,ref Drone drone,in Drone.TargetPlant _targetPlant ,in Translation translation) =>
				{
					ref readonly var smoothPosition = ref translation.Value;
					ref var position = ref drone.Position;
					var targetPos = new float3(position.x, drone.HoverHeight, position.z);
					var targetPlantEntity = _targetPlant.Value;
					var targetPlant = GetComponent<Plant>(targetPlantEntity);
					if (targetPlant.harvested)
					{
						parallelCommandBuffer.RemoveComponent<Drone.TargetPlant>(entityInQueryIndex, entity);
					}
					else
					{
						targetPos.xz = (float2)targetPlant.Position + 0.5f;
                        if (math.distancesq(targetPos.xz,position.xz)<3f*3f)
                        {
							targetPos.y = 0f;
                        }
						if(math.all((int2)smoothPosition.xz == targetPlant.Position) & position.y < 0.5f)
                        {
							parallelCommandBuffer.AddComponent(entityInQueryIndex, entity, new Drone.SellPlant() { HeldPlant = targetPlantEntity });
							targetPlant.harvested = true;
							parallelCommandBuffer.RemoveComponent<Drone.TargetPlant>(entityInQueryIndex, entity);
							Helper.HarvestPlant(farm, tiles, GetComponentDataFromEntity<GroundTile>(), GetComponentDataFromEntity<Plant>(), (int2)smoothPosition.xz);
						}
					}
					position.x = Mathf.MoveTowards(position.x, targetPos.x, Drone.xzSpeed * deltaTime);
					position.y = Mathf.MoveTowards(position.y, targetPos.y, deltaTime * Drone.ySpeed);
					position.z = Mathf.MoveTowards(position.z, targetPos.z, Drone.xzSpeed * deltaTime);
				}).Schedule();
			commandBufferSystem.AddJobHandleForProducer(Dependency);
		}
	}
    [UpdateInGroup(typeof(DroneSimulationSystemGroup))]
	public class SearchStoreSystem : SystemBase
	{
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
		{
			
			var farm = GetSingleton<Farm>();
			var tiles = GetBuffer<Farm.GroundTiles>(GetSingletonEntity<Farm.GroundTiles>()).AsNativeArray().Reinterpret<Entity>();
			var deltaTime = Time.DeltaTime;
			var droneGlobalParams = GetSingleton<DroneGlobalParams>();
			var carrySmooth = 1f - math.pow(droneGlobalParams.CarrySmooth, deltaTime);
			var commandBuffer = commandBufferSystem.CreateCommandBuffer();
			var pathing = new Pathing(farm, tiles, GetComponentDataFromEntity<GroundTile>(true),Allocator.TempJob);

			Entities
				.WithAll<Translation>()
				.WithNone<Drone.SellPlant.StorePosition>()
				.ForEach((Entity entity, int entityInQueryIndex, in Drone drone, in Drone.SellPlant sellPlant) =>
				{
					var smoothPosition = GetComponent<Translation>(entity).Value;
					ref readonly var position = ref drone.Position;
					var targetPos = new float3(position.x, drone.HoverHeight, position.z);
					var plantPos = smoothPosition;
					plantPos.y += 0.08f;
					Helper.EaseToWorldPosition(GetComponentDataFromEntity<Translation>(), sellPlant.HeldPlant, plantPos, carrySmooth);

					
					int storeTileHash = pathing.SearchForOne((int2)smoothPosition.xz, 30,new Pathing.IsNavigableAll(),new Pathing.IsStore(), pathing.FullMapZone);
					if (storeTileHash != -1)
					{
						var storePos = pathing.Unhash(storeTileHash);
						commandBuffer.AddComponent(entity, new Drone.SellPlant.StorePosition() { Value = storePos });
					}
					

				}).Schedule();
			pathing.Dispose(Dependency);
			commandBufferSystem.AddJobHandleForProducer(Dependency);
		}
	}
	[UpdateInGroup(typeof(DroneSimulationSystemGroup))]
	public class SellPlantSystem : SystemBase
	{
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
		{
			var farmEntity = GetSingletonEntity<Farm>();
			var deltaTime = Time.DeltaTime;
			var droneGlobalParams = GetSingleton<DroneGlobalParams>();
			var carrySmooth = 1f - math.pow(droneGlobalParams.CarrySmooth, deltaTime);
			var commandBuffer = commandBufferSystem.CreateCommandBuffer();
			Entities
				.WithAll<Translation>()
				.ForEach((Entity entity, ref Drone drone, in Drone.SellPlant sellPlant,in Drone.SellPlant.StorePosition _storePosition) =>
				{
					var smoothPosition = GetComponent<Translation>(entity).Value;
					ref var position = ref drone.Position;
					ref readonly var storePosition = ref _storePosition.Value;
					var targetPos = new float3(position.x, drone.HoverHeight, position.z);
					var plantPos = smoothPosition;
					plantPos.y += 0.08f;
					Helper.EaseToWorldPosition(GetComponentDataFromEntity<Translation>(), sellPlant.HeldPlant, plantPos, carrySmooth);
					if(math.all((int2)smoothPosition.xz == storePosition))
                    {
						Helper.SellPlant(farmEntity, GetComponentDataFromEntity<Money>(), GetComponentDataFromEntity<FarmerGlobalParams>(), GetComponentDataFromEntity<DroneGlobalParams>(), sellPlant.HeldPlant, GetComponentDataFromEntity<Plant>(), storePosition, commandBuffer);
						commandBuffer.RemoveComponent<Drone.SellPlant>(entity);
						commandBuffer.RemoveComponent<Drone.SellPlant.StorePosition>(entity);

                    }
                    else
                    {
						
						position.y = Mathf.MoveTowards(position.y, drone.HoverHeight, Drone.ySpeed * deltaTime);
						position.x = Mathf.MoveTowards(position.x, storePosition.x + .5f, Drone.xzSpeed * deltaTime);
						position.z = Mathf.MoveTowards(position.z, storePosition.y + .5f, Drone.xzSpeed * deltaTime);
					}

				}).Schedule();
			commandBufferSystem.AddJobHandleForProducer(Dependency);
		}
	}
	[UpdateInGroup(typeof(DroneSimulationSystemGroup), OrderLast = true)]
	public class DroneTransformSystemGroup : ComponentSystemGroup
    {

    }

    [UpdateInGroup(typeof(DroneTransformSystemGroup))]
    public class DroneSmoothPositionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
			var deltaTime = Time.DeltaTime;
			var droneGlobalParams = GetSingleton<DroneGlobalParams>();
			var moveSmooth = 1f - math.pow(droneGlobalParams.MoveSmooth, deltaTime);
			Entities.ForEach((ref Translation translation, in Drone drone) =>
			{
				translation.Value = math.lerp(translation.Value, drone.Position, moveSmooth);
			}).ScheduleParallel();
        }
    }

	[UpdateInGroup(typeof(DroneTransformSystemGroup))]
	[UpdateAfter(typeof(DroneSmoothPositionSystem))]
	public class DroneRotationSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.ForEach((ref Rotation rotation ,in Translation translation, in Drone drone) =>
			{
				var tilt = new float3(drone.Position.x - translation.Value.x, 2f, drone.Position.z - translation.Value.z);
				rotation.Value = Quaternion.FromToRotation(Vector3.up, tilt);
			}).ScheduleParallel();
		}
	}
}
