using System.Collections.Generic;
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
    [UpdateInGroup(typeof(InitializationSystemGroup))]
	[UpdateAfter(typeof(ConvertToEntitySystem))]
	public class AutoFarmersInitializationSystemGroup : ComponentSystemGroup
    {

    }
	[UpdateInGroup(typeof(AutoFarmersInitializationSystemGroup))]
	public class PlantPrefabInitializationSystem : SystemBase
	{
		EntityQuery uninitializedFarmQuery;
		protected override void OnCreate()
		{
			uninitializedFarmQuery = GetEntityQuery(ComponentType.ReadOnly<Farm>(), ComponentType.Exclude<PlantPrefabs>());
		}
		protected override void OnUpdate()
		{
			using var farmEntities = uninitializedFarmQuery.ToEntityArray(Allocator.Temp);
			using var farms = uninitializedFarmQuery.ToComponentDataArray<Farm>(Allocator.Temp);
			for (int i = 0; i < farmEntities.Length; i++)
			{
				var farmEntity = farmEntities[i];
				var farm = farms[i];
				var basePrefab = farm.PlantPrefab;
				var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(basePrefab);
				var plantPrefabs = EntityManager.AddBuffer<PlantPrefabs>(farmEntity).Reinterpret<Entity>();
				for (int ii = 0; ii < Plant.Variants; ii++)
				{
					var prefab = EntityManager.Instantiate(basePrefab);
					EntityManager.AddComponent<Prefab>(prefab);
					var seed = farm.SeedOffset + ii;
					renderMesh.mesh = GenerateMesh(seed); EntityManager.SetSharedComponentData(prefab, renderMesh);
					GetBuffer<PlantPrefabs>(farmEntity).Add(new PlantPrefabs() { Value = prefab });
				}
			}
		}
		readonly List<Vector3> vertices = new List<Vector3>();
		readonly List<int> triangles = new List<int>();
		readonly List<Color> colors = new List<Color>();
		readonly List<Vector2> uv = new List<Vector2>();
		Mesh GenerateMesh(int seed)
		{
			UnityEngine.Random.State oldRandState = UnityEngine.Random.state;
			UnityEngine.Random.InitState(seed);

			vertices.Clear();
			triangles.Clear();
			colors.Clear();
			uv.Clear();

			Color color1 = UnityEngine.Random.ColorHSV(0f, 1f, .5f, .8f, .25f, .9f);
			Color color2 = UnityEngine.Random.ColorHSV(0f, 1f, .5f, .8f, .25f, .9f);

			float height = UnityEngine.Random.Range(.4f, 1.4f);

			float angle = UnityEngine.Random.value * Mathf.PI * 2f;
			float armLength1 = UnityEngine.Random.value * .4f + .1f;
			float armLength2 = UnityEngine.Random.value * .4f + .1f;
			float armRaise1 = UnityEngine.Random.value * .3f;
			float armRaise2 = UnityEngine.Random.value * .6f - .3f;
			float armWidth1 = UnityEngine.Random.value * .5f + .2f;
			float armWidth2 = UnityEngine.Random.value * .5f + .2f;
			float armJitter1 = UnityEngine.Random.value * .3f;
			float armJitter2 = UnityEngine.Random.value * .3f;
			float stalkWaveStr = UnityEngine.Random.value * .5f;
			float stalkWaveFreq = UnityEngine.Random.Range(.25f, 1f);
			float stalkWaveOffset = UnityEngine.Random.value * Mathf.PI * 2f;

			int triCount = UnityEngine.Random.Range(15, 35);

			for (int i = 0; i < triCount; i++)
			{
				// front face
				triangles.Add(vertices.Count);
				triangles.Add(vertices.Count + 1);
				triangles.Add(vertices.Count + 2);

				// back face
				triangles.Add(vertices.Count + 1);
				triangles.Add(vertices.Count);
				triangles.Add(vertices.Count + 2);

				float t = i / (triCount - 1f);
				float armLength = Mathf.Lerp(armLength1, armLength2, t);
				float armRaise = Mathf.Lerp(armRaise1, armRaise2, t);
				float armWidth = Mathf.Lerp(armWidth1, armWidth2, t);
				float armJitter = Mathf.Lerp(armJitter1, armJitter2, t);
				float stalkWave = Mathf.Sin(t * stalkWaveFreq * 2f * Mathf.PI + stalkWaveOffset) * stalkWaveStr;

				float y = t * height;
				vertices.Add(new Vector3(stalkWave, y, 0f));
				Vector3 armPos = new Vector3(stalkWave + Mathf.Cos(angle) * armLength, y + armRaise, Mathf.Sin(angle) * armLength);
				vertices.Add(armPos + UnityEngine.Random.insideUnitSphere * armJitter);
				armPos = new Vector3(stalkWave + Mathf.Cos(angle + armWidth) * armLength, y + armRaise, Mathf.Sin(angle + armWidth) * armLength);
				vertices.Add(armPos + UnityEngine.Random.insideUnitSphere * armJitter);

				colors.Add(color1);
				colors.Add(color2);
				colors.Add(color2);
				uv.Add(Vector2.zero);
				uv.Add(Vector2.right);
				uv.Add(Vector2.right);

				// golden angle in radians
				angle += 2.4f;
			}

			Mesh outputMesh = new Mesh();
			outputMesh.name = "Generated Plant (" + seed + ")";

			outputMesh.SetVertices(vertices);
			outputMesh.SetColors(colors);
			outputMesh.SetTriangles(triangles, 0);
			outputMesh.RecalculateNormals();

			UnityEngine.Random.state = oldRandState;
			return outputMesh;
		}
	}
	[UpdateInGroup(typeof(AutoFarmersInitializationSystemGroup))]
	public class CreateFarmGroundTilesSystem : SystemBase
	{
		EntityQuery farmQuery;
		protected override void OnCreate()
		{
			farmQuery = GetEntityQuery(ComponentType.ReadOnly<Farm>(), ComponentType.Exclude<Farm.GroundTiles>());
		}

		protected override void OnUpdate()
		{
			if (farmQuery.CalculateChunkCount() > 0)
			{
				var farmEntity = farmQuery.GetSingletonEntity();
				var farm = farmQuery.GetSingleton<Farm>();
				var groundTilePrefab = farm.GroundTilePrefab;
				var mapSize = farm.MapSize;
				EntityManager.Instantiate(groundTilePrefab, mapSize.x * mapSize.y, Allocator.Temp).Dispose();
				var tiles = EntityManager.AddBuffer<Farm.GroundTiles>(farmEntity);
				tiles.Length = mapSize.x * mapSize.y;

				Entities
					.WithNativeDisableParallelForRestriction(tiles)
					.ForEach((Entity entity, int entityInQueryIndex, ref GroundTile tile, ref GroundTile.Tilled tilled, ref Translation translation) =>
					{
						var random = Random.CreateFromIndex((uint)entityInQueryIndex);
						Helper.HashToPosition(farm, entityInQueryIndex, out var x, out var y);
						tile.Position = new int2(x, y);
						translation.Value = new float3(x + 0.5f, 0, y + 0.5f);
						tilled.Value = random.NextFloat(0.2f);
						tiles[entityInQueryIndex] = entity;
					}).ScheduleParallel();
			}
		}
	}
	[UpdateInGroup(typeof(AutoFarmersInitializationSystemGroup))]
	[UpdateAfter(typeof(CreateFarmGroundTilesSystem))]
	public class SpawnStoreSystem : SystemBase
	{
		EntityQuery query;
		protected override void OnUpdate()
		{
			var entityManager = EntityManager;

			Entities.WithStoreEntityQueryInField(ref query)
				.WithStructuralChanges()
				.ForEach((Entity entity, int entityInQueryIndex, in Farm farm, in Farm.StoreCount storeCount) =>
				{
					var tiles = GetBuffer<Farm.GroundTiles>(entity).AsNativeArray().Reinterpret<Entity>();
					var random = Random.CreateFromIndex((uint)entityInQueryIndex);
					var spawnedStores = 0;
					while (spawnedStores < storeCount.Value)
					{
						var pos = random.NextInt2(farm.MapSize);
						var tileEntity = tiles[Helper.PositionToHash(farm, pos)];
						var tile = entityManager.GetComponentData<GroundTile>(tileEntity);
						if (!tile.StoreTile)
						{
							tile.StoreTile = true;
							entityManager.SetComponentData(tileEntity, tile);
							var store = entityManager.Instantiate(farm.StorePrefab);
							entityManager.SetComponentData(store, new Translation() { Value = new float3(pos.x + 0.5f, 0.6f, pos.y + 0.5f) });
							tiles = entityManager.GetBuffer<Farm.GroundTiles>(entity).AsNativeArray().Reinterpret<Entity>();
							spawnedStores++;
						}
					}
				}).Run();
			entityManager.RemoveComponent<Farm.StoreCount>(query);
		}
	}
	[UpdateInGroup(typeof(AutoFarmersInitializationSystemGroup))]
	[UpdateAfter(typeof(SpawnStoreSystem))]
	public class SpawnRocksSystem : SystemBase
	{
		EntityQuery query;
		protected override void OnUpdate()
		{
			var entityManager = EntityManager;

            Entities.WithStoreEntityQueryInField(ref query)
				.WithStructuralChanges()
				.ForEach((Entity entity, int entityInQueryIndex, in Farm farm, in Farm.RockSpawnAttempts rockSpawnAttempts) =>
				{
					var tiles = GetBuffer<Farm.GroundTiles>(entity).AsNativeArray().Reinterpret<Entity>();
					var random = Random.CreateFromIndex((uint)entityInQueryIndex);
					for (int spawnIndex = 0; spawnIndex < rockSpawnAttempts.Value; spawnIndex++)
					{
						var size = random.NextInt2(4);
						var pos = random.NextInt2(farm.MapSize - size);
						var rect = new RectInt(pos.x, pos.y, size.x, size.y);
						for (int y = rect.yMin; y <= rect.yMax; y++)
						{
							for (int x = rect.xMin; x <= rect.xMax; x++)
							{
								var tileEntity = tiles[Helper.PositionToHash(farm, new int2(x, y))];
								var tile = entityManager.GetComponentData<GroundTile>(tileEntity);
								if (tile.TileRock != Entity.Null | tile.StoreTile)
								{
									goto NextRock;
								}
							}
						}
						var rockEntity = entityManager.Instantiate(farm.RockPrefab);
                        int health = (rect.width + 1) * (rect.height + 1) * 15;
                        var rock = new Rock()
						{
							Rect = rect,
							Depth = random.NextFloat(0.4f, 0.8f),
							Health = health,
							StartHealth = health,
						};
						var center2D = rect.center;
						entityManager.SetComponentData(rockEntity, rock);
						entityManager.SetComponentData(rockEntity, new Translation() { Value = new float3(center2D.x + 0.5f, rock.Depth * 0.5f, center2D.y + 0.5f) });
						entityManager.SetComponentData(rockEntity, new NonUniformScale() { Value = new float3(rect.width + .5f, rock.Depth, rect.height + 0.5f) });
						tiles = entityManager.GetBuffer<Farm.GroundTiles>(entity).AsNativeArray().Reinterpret<Entity>();
						for (int y = rect.yMin; y <= rect.yMax; y++)
						{
							for (int x = rect.xMin; x <= rect.xMax; x++)
							{
                                int2 tilePosition = new int2(x, y);
								var tileEntity = tiles[Helper.PositionToHash(farm, tilePosition)];
								var tile = entityManager.GetComponentData<GroundTile>(tileEntity);

								tile.TileRock = rockEntity;
								entityManager.SetComponentData(tileEntity, tile);
							}
						}
					NextRock:;
					}


				}).Run();
			entityManager.RemoveComponent<Farm.RockSpawnAttempts>(query);
		}
	}

	[UpdateInGroup(typeof(AutoFarmersInitializationSystemGroup), OrderLast = true)]
	public class SpawnFarmerSystem : SystemBase
	{
		EntityQuery query;
		EntityCommandBufferSystem commandBufferSystem;
		protected override void OnCreate()
		{
			commandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
		}
		protected override void OnUpdate()
		{
			var entityManager = EntityManager;
			var farm = GetSingleton<Farm>();
			var farmerGlobalParams = GetSingleton<FarmerGlobalParams>();
			var farmerPrefab = farmerGlobalParams.FarmerPrefab;
			var tiles = GetBuffer<Farm.GroundTiles>(GetSingletonEntity<Farm>()).AsNativeArray().Reinterpret<Entity>();

			var parallelCommandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
			Entities.WithStoreEntityQueryInField(ref query).WithReadOnly(tiles)
				.ForEach((Entity entity, int entityInQueryIndex, in SpawnFarmer spawnFarmer) =>
				{
					var random = Random.CreateFromIndex((uint)entityInQueryIndex);
					for (int spawnIndex = 0; spawnIndex < spawnFarmer.Count; spawnIndex++)
					{
						for (int i = 0; i < 100; i++)
						{
							var spawnPos = random.NextInt2(farm.MapSize);
							if (GetComponent<GroundTile>(tiles[Helper.PositionToHash(farm, spawnPos)]).TileRock == Entity.Null)
							{
								var farmerEntity = parallelCommandBuffer.Instantiate(entityInQueryIndex, farmerPrefab);
								var pos3D = new float3(spawnPos.x + 0.5f, 0.5f, spawnPos.y + 0.5f);
								parallelCommandBuffer.SetComponent(entityInQueryIndex, farmerEntity, new Farmer() { Position = pos3D.xz });
								parallelCommandBuffer.SetComponent(entityInQueryIndex, farmerEntity, new Translation() { Value = pos3D});
								break;
							}
						}
					}


				}).ScheduleParallel();
			commandBufferSystem.CreateCommandBuffer().RemoveComponent<SpawnFarmer>(query);
			commandBufferSystem.AddJobHandleForProducer(Dependency);
		}
	}

}