using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
        var pathMovement = World.GetExistingSystem<PathMovement>();
        var random = new Random(1234);

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
					   pathMovement.WalkTo(farmerPosition.x, farmerPosition.y, 40, tileBuffer, pathMovement.isStore, pathNodes);
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
						   int tileHash = pathMovement.SearchForOne(farmerPosition.x, farmerPosition.y, 25, tileBuffer,pathMovement.defaultNavigation, pathMovement.isReadyToPlant, pathMovement.fullMapZone);
						   if (tileHash != -1)
						   {
							   int tileX, tileY;
							   pathMovement.Unhash(tileHash, out tileX, out tileY);
							   pathMovement.AssignLatestPath(pathNodes, tileX, tileY);
						   }
						   else
						   {
							   ecb.RemoveComponent<PlantCropIntention>(entity);
						   }
					   }
				   }
			   }


		   }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
    }
}