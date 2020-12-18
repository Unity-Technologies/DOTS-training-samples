using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FarmerIntentionSystem))]
public class PlantSystem : SystemBase
{
	private EndSimulationEntityCommandBufferSystem m_ECB;

	protected override void OnCreate()
	{
		base.OnCreate();

		m_ECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
	{
		var parallelEcb = m_ECB.CreateCommandBuffer().AsParallelWriter();
		var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();

		var tileBufferHandler = GetBufferFromEntity<TileState>(true);
        var random = new Unity.Mathematics.Random(1234);
		var isStore = PathSystem.isStore;
		var isReadyToPlant = PathSystem.isReadyToPlant;
		var defaultNavigation = PathSystem.defaultNavigation;
		var fullMapZone = new RectInt(0, 0, settings.GridSize.x, settings.GridSize.y);


		Entities
		   .WithAll<Farmer>()
		   .WithAll<PlantCropIntention>()
		   .WithNone<HasSeeds>()
		   .WithReadOnly(tileBufferHandler)
		   .ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<PathNode> pathNodes, in Translation translation) =>
		   {
			   var tileBuffer = tileBufferHandler[data];
			   var farmerPosition = new int2((int)math.floor(translation.Value.x), (int)math.floor(translation.Value.z));
			   var farmerLinearIndex = farmerPosition.x + farmerPosition.y * settings.GridSize.x;
			   var state = tileBuffer[farmerLinearIndex].Value;

			   if (state == ETileState.Store)
			   {
				   parallelEcb.AddComponent<HasSeeds>(entityInQueryIndex, entity);
			   }
			   else if (pathNodes.Length == 0)
			   {
				   PathSystem.WalkTo(farmerPosition.x, farmerPosition.y, 40, tileBuffer, defaultNavigation, isStore, pathNodes, fullMapZone);
				   if (pathNodes.Length == 0)
				   {
					   parallelEcb.RemoveComponent<PlantCropIntention>(entityInQueryIndex, entity);
				   }
			   }
		   }).ScheduleParallel();

		var ecb = m_ECB.CreateCommandBuffer();

		Entities
           .WithAll<Farmer>()
		   .WithAll<PlantCropIntention>()
		   .WithAll<HasSeeds>()
		   .WithReadOnly(tileBufferHandler)
           .ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<PathNode> pathNodes, in Translation translation) =>
           {
			   var tileBuffer = tileBufferHandler[data];
			   var farmerPosition = new int2((int)math.floor(translation.Value.x), (int)math.floor(translation.Value.z));
			   var farmerLinearIndex = farmerPosition.x + farmerPosition.y * settings.GridSize.x;
			   var state = tileBuffer[farmerLinearIndex].Value;

			   if (state == ETileState.Tilled)
			   {
				   pathNodes.Clear();
				   tileBuffer[farmerLinearIndex] = new TileState { Value = ETileState.Seeded };
			   }
			   else
			   {
				   if (pathNodes.Length == 0)
				   {
					   if (random.NextFloat() < .1f)
					   {
						   parallelEcb.RemoveComponent<PlantCropIntention>(entityInQueryIndex, entity);
						   parallelEcb.RemoveComponent<HasSeeds>(entityInQueryIndex, entity);
					   }
					   else
					   {
						   NativeArray<int> visitedTiles;
						   int tileHash = PathSystem.SearchForOne(farmerPosition.x, farmerPosition.y, 25, tileBuffer, defaultNavigation, isReadyToPlant, fullMapZone, fullMapZone, out visitedTiles);
						   if (tileHash != -1)
						   {
							   int tileX, tileY;
							   PathSystem.Unhash(tileHash, fullMapZone, out tileX, out tileY);
							   PathSystem.AssignLatestPath(pathNodes, tileX, tileY, fullMapZone, visitedTiles);
						   }
						   else
						   {
							   parallelEcb.RemoveComponent<PlantCropIntention>(entityInQueryIndex, entity);
							   parallelEcb.RemoveComponent<HasSeeds>(entityInQueryIndex, entity);
						   }
						   visitedTiles.Dispose();
					   }
				   }
			   }


		   }).Schedule();

		m_ECB.AddJobHandleForProducer(Dependency);
	}
}