using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FarmerIntentionSystem))]
public class PlantSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();

		var tileBuffer = GetBufferFromEntity<TileState>()[data];
        var pathBuffers = GetBufferFromEntity<PathNode>();
        var random = new Unity.Mathematics.Random(1234);
		var isStore = PathSystem.isStore;
		var isReadyToPlant = PathSystem.isReadyToPlant;
		var defaultNavigation = PathSystem.defaultNavigation;
		var fullMapZone = new RectInt(0, 0, settings.GridSize.x, settings.GridSize.y);

		Entities
           .WithAll<Farmer>()
           .ForEach((Entity entity, ref PlantCropIntention planCropIntention, in Translation translation) =>
           {
			   var farmerPosition = new int2((int)math.floor(translation.Value.x), (int)math.floor(translation.Value.z));
			   var pathNodes = pathBuffers[entity];
			   var farmerLinearIndex = farmerPosition.x + farmerPosition.y * settings.GridSize.x;
			   var state = tileBuffer[farmerLinearIndex].Value;

			   if (planCropIntention.hasBoughtSeeds == false)
			   {
				   if (state == ETileState.Store) 
				   {
					   planCropIntention.hasBoughtSeeds = true;
				   }
				   else if (pathNodes.Length == 0)
				   {
					   PathSystem.WalkTo(farmerPosition.x, farmerPosition.y, 40, tileBuffer, defaultNavigation, isStore, pathNodes, fullMapZone);
					   if (pathNodes.Length == 0)
					   {
						   ecb.RemoveComponent<PlantCropIntention>(entity);
					   }
				   }
			   }
			   else if (state == ETileState.Tilled)
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
						   ecb.RemoveComponent<PlantCropIntention>(entity);
					   }
					   else
					   {
						   NativeArray<int> visitedTiles;
						   int tileHash = PathSystem.SearchForOne(farmerPosition.x, farmerPosition.y, 25, tileBuffer,defaultNavigation, isReadyToPlant, fullMapZone, fullMapZone, out visitedTiles);
						   if (tileHash != -1)
						   {
							   int tileX, tileY;
							   PathSystem.Unhash(tileHash, fullMapZone, out tileX, out tileY);
							   PathSystem.AssignLatestPath(pathNodes, tileX, tileY, fullMapZone, visitedTiles);
						   }
						   else
						   {
							   ecb.RemoveComponent<PlantCropIntention>(entity);
						   }
						   visitedTiles.Dispose();
					   }
				   }
			   }


		   }).Run();

        ecb.Playback(EntityManager);
    }
}