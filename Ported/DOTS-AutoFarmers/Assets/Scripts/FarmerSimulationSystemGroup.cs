using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace AutoFarmers
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
	public class FarmerSimulationSystemGroup : ComponentSystemGroup
	{

	}


    [UpdateInGroup(typeof(FarmerSimulationSystemGroup))]
    public class IntensionSystemGroup : ComponentSystemGroup
    {

    }

	[UpdateInGroup(typeof(IntensionSystemGroup))]
	public class PickNewIntensionSystem : SystemBase
	{
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
		{
			var globalSeed = Time.ElapsedTime.GetHashCode();
			var parallelCommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
			Entities
			.WithNone<Intension.SmashRocks>()
			.WithNone<Intension.TillGround>()
			.WithNone<Intension.PlantSeeds>()
			.WithNone<Intension.SellPlants>()
			.WithAll<Farmer>()
			.ForEach((Entity entity, int entityInQueryIndex) =>
			{
				var componentType = ((globalSeed^entityInQueryIndex) & 3) switch
				{
					0 => ComponentType.ReadWrite<Intension.SmashRocks>(),
					1 => ComponentType.ReadWrite<Intension.TillGround>(),
					2 => ComponentType.ReadWrite<Intension.PlantSeeds>(),
					3 => ComponentType.ReadWrite<Intension.SellPlants>(),
					_ => throw new System.Exception(),
				};
				parallelCommandBuffer.AddComponent(entityInQueryIndex, entity, componentType);
			}).ScheduleParallel();
			commandBufferSystem.AddJobHandleForProducer(Dependency);
		}


	}
	[UpdateInGroup(typeof(IntensionSystemGroup))]
	public class SmashRocksSystem : SystemBase
	{
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
		{
			var globalSeed = Time.ElapsedTime.GetHashCode();
			var farm = GetSingleton<Farm>();
			var tiles = GetBuffer<Farm.GroundTiles>(GetSingletonEntity<Farm.GroundTiles>()).AsNativeArray().Reinterpret<Entity>();
			
			var parallelCommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            Entities
                .WithReadOnly(tiles)
			.ForEach((Entity entity, int entityInQueryIndex, ref Intension.SmashRocks smashRocks, ref DynamicBuffer<Path> path, in Farmer farmer) =>
			{
				var random = Random.CreateFromIndex((uint)(entityInQueryIndex ^ globalSeed));
				ref var targetRock = ref smashRocks.TargetRock;
				ref var attackingARock = ref smashRocks.AttackingARock;
				attackingARock = false;
                Rock rock;
                if (HasComponent<Rock>(targetRock) && (rock = GetComponent<Rock>(targetRock)).Health > 0)
                {
                    if (path.Length == 1)
                    {
                        attackingARock = true;
                        rock.Health -= 1;
                        SetComponent(targetRock, rock);
						var scale = GetComponent<NonUniformScale>(targetRock).Value;
						scale.y = rock.Depth * ((float)rock.Health / rock.StartHealth) + random.NextFloat(0.1f);
						var pos = GetComponent<Translation>(targetRock).Value;
						pos.y = scale.y * 0.5f;
						SetComponent(targetRock, new NonUniformScale() { Value = scale });
						SetComponent(targetRock, new Translation() { Value = pos });
                    }
                    if (rock.Health <= 0)
                    {
						var rect = rock.Rect;
						for (int y = rect.yMin; y <= rect.yMax; y++)
						{
							for (int x = rect.xMin; x <= rect.xMax; x++)
							{
                                var tilePosition = new int2(x, y);
                                var tileEntity = tiles[Helper.PositionToHash(farm, tilePosition)];
                                var tile = GetComponent<GroundTile>(tileEntity);

								tile.TileRock = Entity.Null;
                                SetComponent(tileEntity, tile);
                            }
                        }
                        parallelCommandBuffer.DestroyEntity(entityInQueryIndex, targetRock);
                        parallelCommandBuffer.RemoveComponent<Intension.SmashRocks>(entityInQueryIndex, entity);
                    }

                }
                else
                {
					var pathing = new Pathing(farm, tiles, GetComponentDataFromEntity<GroundTile>(true));
					targetRock = pathing.FindNearbyRock((int2)farmer.Position, 20, path);
					pathing.Dispose();
                    if (targetRock == Entity.Null)
                    {
                        parallelCommandBuffer.RemoveComponent<Intension.SmashRocks>(entityInQueryIndex, entity);
                    }
                }



            }).Schedule();
			commandBufferSystem.AddJobHandleForProducer(Dependency);
		}
	}
	[UpdateInGroup(typeof(IntensionSystemGroup))]
	public class TillGroundSystem : SystemBase
	{
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
		{
			var globalSeed = Time.ElapsedTime.GetHashCode();
			var farm = GetSingleton<Farm>();
			var tiles = GetBuffer<Farm.GroundTiles>(GetSingletonEntity<Farm.GroundTiles>()).AsNativeArray().Reinterpret<Entity>();

			var parallelCommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
			Entities
			.WithReadOnly(tiles)
			.WithAll<Intension.TillGround>()
			.WithNone<Intension.TillGround.FoundTillingZone>()
			.ForEach((Entity entity, int entityInQueryIndex, in Farmer farmer) =>
			{
				ref readonly var position = ref farmer.Position;
				var random = Random.CreateFromIndex((uint)(entityInQueryIndex^globalSeed));
				{

					var size = random.NextInt2(1, 8);
					var min = (int2)position + random.NextInt2(-10, 10 - size);
					min = math.max(0, min);
					min = math.min(farm.MapSize - size - 1, min);


					for (int _x = 0; _x <= size.x; _x++)
					{
						var x = _x + min.x;
						for (int _y = 0; _y <= size.y; _y++)
						{
							var y = _y + min.y;
							var index = x + farm.MapSize.x * y;
							var tile = GetComponent<GroundTile>(tiles[index]);
							var groundState = tile.State;
							if ((groundState != GroundState.Default & groundState != GroundState.Tilled)
							| (tile.TileRock != Entity.Null | tile.StoreTile))
							{
								if (random.NextFloat() < 0.2f)
								{
									parallelCommandBuffer.RemoveComponent<Intension.TillGround>(entityInQueryIndex, entity);
								}
								return;

							}
						}

					}
					parallelCommandBuffer.AddComponent(entityInQueryIndex, entity, new Intension.TillGround.FoundTillingZone() { TillingZone = new RectInt(min.x, min.y, size.x, size.y) });

				}




			}).ScheduleParallel();
			parallelCommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
			
			Entities
			.WithReadOnly(tiles)
		   .WithAll<Intension.TillGround>()
		   .ForEach((Entity entity, int entityInQueryIndex, ref Intension.TillGround.FoundTillingZone foundTillingZone, ref DynamicBuffer<Path> path, in Farmer farmer) =>
		   {
			   ref readonly var position = ref farmer.Position;
			   ref var tillingZone = ref foundTillingZone.TillingZone;
			   var random = Random.CreateFromIndex((uint)(entityInQueryIndex ^ globalSeed));
			   {

				   var x = (int)position.x;
				   var y = (int)position.y;
				   {
					   var index = x + farm.MapSize.x * y;
					   var tileEntity = tiles[index];
					   var tile = GetComponent<GroundTile>(tileEntity);
					   if (tile.State == GroundState.Default)
					   {
						   if (x >= tillingZone.xMin && x <= tillingZone.xMax)
						   {
							   if (y >= tillingZone.yMin && y <= tillingZone.yMax)
							   {
								   path.Clear();
								   tile.State = GroundState.Tilled;
								   SetComponent(tileEntity, tile);
								   SetComponent(tileEntity, new GroundTile.Tilled() { Value = random.NextFloat(0.8f, 1f) });
							   }
						   }
					   }
					   if (path.Length == 0)
					   {
						   var pathing = new Pathing(farm, tiles, GetComponentDataFromEntity<GroundTile>(true));
						   int tileHash = pathing.SearchForOne((int2)position, 25,new Pathing.IsNavigableDefault(),new Pathing.IsTillable(), tillingZone);
						   if (tileHash != -1)
						   {
							   pathing.AssignLatestPath(path,pathing.Unhash(tileHash));
						   }
						   else
						   {
							   parallelCommandBuffer.RemoveComponent<Intension.TillGround>(entityInQueryIndex, entity);
							   parallelCommandBuffer.RemoveComponent<Intension.TillGround.FoundTillingZone>(entityInQueryIndex, entity);
						   }
						   pathing.Dispose();
					   }
				   }
			   }



		   }).Schedule();
			commandBufferSystem.AddJobHandleForProducer(Dependency);
		}
	}
	[UpdateInGroup(typeof(IntensionSystemGroup))]
	public class PlantSeedsSystem : SystemBase
	{
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
		{
			var plantPrefabsEntity = GetSingletonEntity<PlantPrefabs>();
			var plantPrefabs = GetBuffer<PlantPrefabs>(plantPrefabsEntity).AsNativeArray().Reinterpret<Entity>();
			var farm = GetSingleton<Farm>();
            var farmEntity = GetSingletonEntity<Farm.GroundTiles>();
            var tiles = GetBuffer<Farm.GroundTiles>(farmEntity).AsNativeArray().Reinterpret<Entity>();

			var parallelCommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
			Entities
			.WithReadOnly(tiles)
			.WithAll<Intension.PlantSeeds>()
			.WithNone<Intension.PlantSeeds.HasBoughtSeeds>()
			.ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<Path> path, in Farmer farmer) =>
			{
				ref readonly var position = ref farmer.Position;
				var index = (int)position.x + farm.MapSize.x * (int)position.y;
				var tile = GetComponent<GroundTile>(tiles[index]);
				if (tile.StoreTile)
				{
					parallelCommandBuffer.AddComponent<Intension.PlantSeeds.HasBoughtSeeds>(entityInQueryIndex, entity);
				}
				else if (path.Length == 0)
				{
					var pathing = new Pathing(farm, tiles, GetComponentDataFromEntity<GroundTile>(true));
					pathing.WalkTo((int2)position, 40,new Pathing.IsStore(), path);
					if (path.Length == 0)
					{
						parallelCommandBuffer.RemoveComponent<Intension.PlantSeeds>(entityInQueryIndex, entity);
					}
					pathing.Dispose();
				}
			}).ScheduleParallel();
			CompleteDependency();
			var entityManager = EntityManager;
			var pathing = new Pathing.StructualChange(farmEntity, entityManager,Allocator.TempJob);
			Entities
			.WithStructuralChanges()
			.WithAll<Intension.PlantSeeds, Intension.PlantSeeds.HasBoughtSeeds,Path>()
			.ForEach((Entity entity, int entityInQueryIndex, in Farmer farmer) =>
			{
				var tiles = entityManager.GetBuffer<Farm.GroundTiles>(farmEntity).AsNativeArray().Reinterpret<Entity>();
				var path = entityManager.GetBuffer<Path>(entity);
				ref readonly var position = ref farmer.Position;
				int tileX = (int)position.x;
				int tileY = (int)position.y;
				var index = tileX + farm.MapSize.x * tileY;
				var tile = entityManager.GetComponentData<GroundTile>(tiles[index]);
				var random = Random.CreateFromIndex((uint)entityInQueryIndex);
				if (tile.State == GroundState.Tilled)
				{
					path.Clear();
					int seed = math.clamp((int)(Mathf.PerlinNoise(tileX / 10f, tileY / 10f) * Plant.Variants),0,Plant.Variants-1);

					Helper.SpawnPlant(entityManager,farmEntity,(int2)position ,seed );
				}
				else
				{
					if (path.Length == 0)
					{
						if (random.NextFloat() < .1f)
						{
							entityManager.RemoveComponent(entity,new ComponentTypes(ComponentType.ReadWrite<Intension.PlantSeeds>(),ComponentType.ReadWrite<Intension.PlantSeeds.HasBoughtSeeds>()));
						}
						else
						{
							
							int tileHash = pathing.SearchForOne((int2)position, 25,new Pathing.IsNavigableDefault(),new Pathing.IsReadyForPlant(), pathing.FullMapZone);
							if (tileHash != -1)
							{
								pathing.AssignLatestPath(path, pathing.Unhash(tileHash));
							}
							else
							{
								entityManager.RemoveComponent(entity, new ComponentTypes(ComponentType.ReadWrite<Intension.PlantSeeds>(), ComponentType.ReadWrite<Intension.PlantSeeds.HasBoughtSeeds>()));
							}
						}
					}
				}
			}).Run();
			pathing.Dispose(Dependency);
			commandBufferSystem.AddJobHandleForProducer(Dependency);

		}
	}
	[UpdateInGroup(typeof(IntensionSystemGroup))]
	public class SellPlantsSystem : SystemBase
	{
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
		{
			var moneyEntity = GetSingletonEntity<Money>();
			var farm = GetSingleton<Farm>();
			var smooth = 1f - math.pow(GetSingleton<FarmerGlobalParams>().MovementSmooth, Time.DeltaTime);
			var tiles = GetBuffer<Farm.GroundTiles>(GetSingletonEntity<Farm.GroundTiles>()).AsNativeArray().Reinterpret<Entity>();
			
			var commandBuffer = commandBufferSystem.CreateCommandBuffer();
			Entities
				.WithReadOnly(tiles)
			.WithAll<Intension.SellPlants>()
			.WithNone<Intension.SellPlants.HeldPlant>()
			.ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<Path> path, in Farmer farmer) =>
			{
				ref readonly var position = ref farmer.Position;
				var index = (int)position.x + farm.MapSize.x * (int)position.y;
				var tile = GetComponent<GroundTile>(tiles[index]);
				if (tile.TilePlant != Entity.Null && GetComponent<Plant.Growth>(tile.TilePlant).Value >= 1f)
				{
					commandBuffer.AddComponent( entity, new Intension.SellPlants.HeldPlant() { Plant = tile.TilePlant });
                    Helper.HarvestPlant(farm,tiles,GetComponentDataFromEntity<GroundTile>(),GetComponentDataFromEntity<Plant>(),(int2)position);
					path.Clear();
				}
				else if (path.Length == 0)
				{
					var pathing = new Pathing(farm, tiles, GetComponentDataFromEntity<GroundTile>(true));
					pathing.WalkTo((int2)position, 25, new Farm.IsHarvestableAndUnreserved(GetComponentDataFromEntity<Plant>(true),GetComponentDataFromEntity<Plant.Growth>(true)), path);
					pathing.Dispose();
					if (path.Length == 0)
					{
						commandBuffer.RemoveComponent<Intension.SellPlants>(entity);
					}
					else
					{
						var pos = path[0].Position;
						var i = pos.x + farm.MapSize.x * pos.y;
                        Entity plantEntity = GetComponent<GroundTile>(tiles[i]).TilePlant;
                        var plant = GetComponent<Plant>(plantEntity);
						plant.reserved = true;
						SetComponent(plantEntity, plant);	
					}
					
				}
			}).Schedule();
			
			commandBuffer = commandBufferSystem.CreateCommandBuffer();
			Entities
				.WithReadOnly(tiles)
			.WithAll<Intension.SellPlants,Translation>()
			.ForEach((Entity entity, int entityInQueryIndex, ref Intension.SellPlants.HeldPlant heldPlant, ref DynamicBuffer<Path> path, in Farmer farmer) =>
			{
				
				ref readonly var position = ref farmer.Position;
				var smoothPosition = GetComponent<Translation>(entity).Value;
				int tileX = (int)position.x;
				int tileY = (int)position.y;
				var index = tileX + farm.MapSize.x * tileY;
				var tile = GetComponent<GroundTile>(tiles[index]);
				var random = Random.CreateFromIndex((uint)entityInQueryIndex);
				Helper.EaseToWorldPosition(GetComponentDataFromEntity<Translation>(), heldPlant.Plant, new float3(smoothPosition.x, 1f, smoothPosition.z), smooth);
				if (tile.StoreTile)
				{
					Helper.SellPlant(moneyEntity, GetComponentDataFromEntity<Money>(),GetComponentDataFromEntity<FarmerGlobalParams>(),GetComponentDataFromEntity<DroneGlobalParams>(), heldPlant.Plant, GetComponentDataFromEntity<Plant>(), (int2)position, commandBuffer);
					commandBuffer.RemoveComponent<Intension.SellPlants.HeldPlant>(entity);
					path.Clear();

				}
				else if (path.Length == 0)
				{
					var pathing = new Pathing(farm, tiles, GetComponentDataFromEntity<GroundTile>(true));
					pathing.WalkTo((int2)position, 40,new Pathing.IsStore(), path);
					pathing.Dispose();
				}
			}).Schedule();
			commandBufferSystem.AddJobHandleForProducer(Dependency);

		}
	}
	[UpdateInGroup(typeof(FarmerSimulationSystemGroup))]
	[UpdateAfter(typeof(IntensionSystemGroup))]
	public class FollowPathSystem : SystemBase
	{

		protected override void OnUpdate()
		{
			var farm = GetSingleton<Farm>();
			var tiles = GetBuffer<Farm.GroundTiles>(GetSingletonEntity<Farm.GroundTiles>()).AsNativeArray().Reinterpret<Entity>();

			var deltaTime = Time.DeltaTime;
			Entities
				.WithReadOnly(tiles)
			.ForEach((ref DynamicBuffer<Path> path, ref Farmer farmer) =>
			{
				ref var position = ref farmer.Position;
				
				if (path.Length > 0)
				{
					var tile = (int2)position;
					var nextTile = path[path.Length - 1].Position;
					if (math.all(tile == nextTile))
					{
						path.Length--;
					}
					else
					{
						if (IsBlocked(nextTile) == false)
						{
							float offset = .5f;
							if (GetComponent<GroundTile>(tiles[PositionToIndex(tile)]).State == GroundState.Plant)
							{
								offset = .01f;
							}
							Vector2 targetPos = (float2)nextTile + offset;
							const float WalkSpeed = 4f;
							position = Vector2.MoveTowards(position, targetPos, WalkSpeed * deltaTime);
						}
					}
				}
				int PositionToIndex(int2 position) => position.x + farm.MapSize.x * position.y;
				bool IsBlocked(int2 position)
                {
					return math.any(position < 0) | math.any(position >= farm.MapSize) || GetComponent<GroundTile>(tiles[PositionToIndex(position)]).TileRock != Entity.Null;
                }

			}).ScheduleParallel();
		}
	}
	[UpdateInGroup(typeof(FarmerSimulationSystemGroup))]
	[UpdateAfter(typeof(FollowPathSystem))]
	public class ApplySmoothPositionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
			var smooth = 1f - math.pow(GetSingleton<FarmerGlobalParams>().MovementSmooth, Time.DeltaTime);
			Entities.ForEach((ref Translation translation, in Farmer farmer) =>
			{
				ref readonly var position = ref farmer.Position;
				ref var smoothPosition = ref translation.Value;
				smoothPosition.xz = math.lerp(smoothPosition.xz, position, smooth);
			}).ScheduleParallel();
        }
    }

	[UpdateInGroup(typeof(FarmerSimulationSystemGroup))]
	[UpdateAfter(typeof(ApplySmoothPositionSystem))]
	public class AttackingARockPositionOffsetSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			var globalSeed = (uint)Time.ElapsedTime.GetHashCode();
			Entities.ForEach((int entityInQueryIndex, ref Translation translation, in Intension.SmashRocks smashRocks, in DynamicBuffer<Path> path, in Farmer farmer) =>
			{
				if (smashRocks.AttackingARock)
				{
					var random = Random.CreateFromIndex((uint)entityInQueryIndex ^ globalSeed);
					ref readonly var position = ref farmer.Position;
					ref var smoothPosition = ref translation.Value;
					smoothPosition.xz += ((float2)path[0].Position + 0.5f - smoothPosition.xz) * random.NextFloat(0.5f);
				}

			}).ScheduleParallel();
		}
	}

}