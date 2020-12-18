using System.Globalization;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TillGroundSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_ECB;
    
    protected override void OnCreate()
    {
        base.OnCreate();

        m_ECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecbParallel = m_ECB.CreateCommandBuffer().AsParallelWriter();

        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();
        
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        var random = new Unity.Mathematics.Random(1234);

        var defaultNavigation = PathSystem.defaultNavigation;
        var isTillable = PathSystem.isTillable;
        var fullMapZone = new RectInt(0, 0, settings.GridSize.x, settings.GridSize.y);

        Dependency = Entities
           .WithAll<Farmer>()
           .WithAll<TillGroundIntention>()
           .WithNone<TillArea>()
           .WithReadOnly(tileBuffer)
           .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
           {
               int width = random.NextInt(1, 8);
               int height = random.NextInt(1, 8);
               int minX = Mathf.FloorToInt(translation.Value.x) + random.NextInt(-10, 10 - width);
               int minY = Mathf.FloorToInt(translation.Value.z) + random.NextInt(-10, 10 - height);
               if (minX < 0) minX = 0;
               if (minY < 0) minY = 0;
               if (minX + width >= settings.GridSize.x) minX = settings.GridSize.x - 1 - width;
               if (minY + height >= settings.GridSize.y) minY = settings.GridSize.y - 1 - height;

               bool blocked = false;
               for (int x = minX; x <= minX + width; x++)
               {
                   for (int y = minY; y <= minY + height; y++)
                   {
                       var linearIndex = x + y * settings.GridSize.x;
                       var state = tileBuffer[linearIndex].Value;

                       if (state != ETileState.Empty && state != ETileState.Tilled)
                       {
                           blocked = true;
                           break;
                       }
                   }
                   if (blocked)
                   {
                       break;
                   }
               }
               if (blocked == false)
               {
                   ecbParallel.AddComponent(entityInQueryIndex, entity, new TillArea { Rect = new RectInt(minX, minY, width, height) });
               }
               else
               {
                   if (random.NextFloat() < 0.2f)
                   {
                       ecbParallel.RemoveComponent<TillGroundIntention>(entityInQueryIndex, entity);
                   }
               }
           }).ScheduleParallel(Dependency);

        m_ECB.AddJobHandleForProducer(Dependency);
        var ecb = m_ECB.CreateCommandBuffer();

        Dependency = Entities
           .WithAll<Farmer>()
           .WithAll<TillGroundIntention>()
           .ForEach((Entity entity, ref DynamicBuffer<PathNode> pathBuffer, in TillArea tillGround, in Translation translation) =>
           {
               var tillingZone = tillGround.Rect;

               Debug.DrawLine(new Vector3(tillingZone.min.x, .1f, tillingZone.min.y), new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.min.y), Color.green);
               Debug.DrawLine(new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.min.y), new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.max.y + 1f), Color.green);
               Debug.DrawLine(new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.max.y + 1f), new Vector3(tillingZone.min.x, .1f, tillingZone.max.y + 1f), Color.green);
               Debug.DrawLine(new Vector3(tillingZone.min.x, .1f, tillingZone.max.y + 1f), new Vector3(tillingZone.min.x, .1f, tillingZone.min.y), Color.green);

               var farmerPosition = new int2(Mathf.FloorToInt(translation.Value.x), Mathf.FloorToInt(translation.Value.z));
               var farmerLinearIndex = farmerPosition.x + farmerPosition.y * settings.GridSize.x;

               bool groundTillable = false;
               if (tileBuffer[farmerLinearIndex].Value == ETileState.Empty)
               {
                   if (farmerPosition.x >= tillingZone.xMin && farmerPosition.x <= tillingZone.xMax)
                   {
                       if (farmerPosition.y >= tillingZone.yMin && farmerPosition.y <= tillingZone.yMax)
                       {
                           groundTillable = true;
                       }
                   }
               }

               if (groundTillable)
               {
                   pathBuffer.Clear();
                   tileBuffer[farmerLinearIndex] = new TileState { Value = ETileState.Tilled };
               }
               else
               {
                   if (pathBuffer.Length == 0)
                   {
                       NativeArray<int> visitedtiles;
                       int tileHash = PathSystem.SearchForOne(farmerPosition.x, farmerPosition.y, 600, tileBuffer, defaultNavigation, isTillable, tillingZone, fullMapZone, out visitedtiles);
                       if (tileHash != -1)
                       {
                           int tileX, tileY;
                           PathSystem.Unhash(tileHash, fullMapZone, out tileX, out tileY);
                           PathSystem.AssignLatestPath(pathBuffer, tileX, tileY, fullMapZone, visitedtiles);
                       }
                       else
                       {
                           ecb.RemoveComponent<TillGroundIntention>(entity);
                       }
                   }
               }
           }).Schedule(Dependency);

        m_ECB.AddJobHandleForProducer(Dependency);
    }
}
