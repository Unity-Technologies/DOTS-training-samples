using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TillGroundSystem))]
public class PlantSystem : SystemBase
{
	private EndSimulationEntityCommandBufferSystem m_ECB;
	
	protected override void OnCreate()
	{
		base.OnCreate();
		GetEntityQuery(typeof(TileState));
		m_ECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
	{
        var parallelEcb = m_ECB.CreateCommandBuffer().AsParallelWriter();
        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();

        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        var random = new Unity.Mathematics.Random((uint)Time.ElapsedTime + 1);
        var isStore = PathSystem.isStore;
        var defaultNavigation = PathSystem.defaultNavigation;
        var fullMapZone = new RectInt(0, 0, settings.GridSize.x, settings.GridSize.y);


        Dependency = Entities
            .WithName("BuySeeds")
           .WithAll<Farmer>()
           .WithAll<PlantCropIntention>()
           .WithNone<HasSeeds>()
           .WithReadOnly(isStore)
           .WithReadOnly(defaultNavigation)
           .WithReadOnly(tileBuffer)
           .ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<PathNode> pathNodes, in Translation translation) =>
           {
               var farmerPosition = new int2((int)math.floor(translation.Value.x), (int)math.floor(translation.Value.z));
               var farmerLinearIndex = farmerPosition.x + farmerPosition.y * settings.GridSize.x;
               var state = tileBuffer[farmerLinearIndex].Value;

               if (state == ETileState.Store)
               {
                   parallelEcb.AddComponent<HasSeeds>(entityInQueryIndex, entity);
               }
               else if (pathNodes.Length == 0)
               {
                   PathSystem.WalkTo(farmerPosition.x, farmerPosition.y, 40, tileBuffer.AsNativeArray(), defaultNavigation, isStore, pathNodes, fullMapZone);
                   if (pathNodes.Length == 0)
                   {
                       parallelEcb.RemoveComponent<PlantCropIntention>(entityInQueryIndex, entity);
                   }
               }
           }).ScheduleParallel(Dependency);

        m_ECB.AddJobHandleForProducer(Dependency);

        var isReadyToPlant = PathSystem.isReadyToPlant;
        var ecb = m_ECB.CreateCommandBuffer();

        Dependency = Entities
           .WithAll<Farmer>()
           .WithAll<PlantCropIntention>()
           .WithAll<HasSeeds>()
           .WithReadOnly(isReadyToPlant)
           .WithReadOnly(defaultNavigation)
           .ForEach((Entity entity, ref DynamicBuffer<PathNode> pathNodes, in Translation translation) =>
           {
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
                           ecb.RemoveComponent<PlantCropIntention>(entity);
                           ecb.RemoveComponent<HasSeeds>(entity);
                       }
                       else
                       {
                           NativeArray<int> visitedTiles;
                           int tileHash = PathSystem.SearchForOne(farmerPosition.x, farmerPosition.y, 25, tileBuffer.AsNativeArray(), defaultNavigation, isReadyToPlant, fullMapZone, fullMapZone, out visitedTiles);
                           if (tileHash != -1)
                           {
                               int tileX, tileY;
                               PathSystem.Unhash(tileHash, fullMapZone, out tileX, out tileY);
                               PathSystem.AssignLatestPath(pathNodes, tileX, tileY, fullMapZone, visitedTiles);
                           }
                           else
                           {
                               ecb.RemoveComponent<PlantCropIntention>(entity);
                               ecb.RemoveComponent<HasSeeds>(entity);
                           }
                           visitedTiles.Dispose();
                       }
                   }
               }


           }).Schedule(Dependency);

        m_ECB.AddJobHandleForProducer(Dependency);
	}
}