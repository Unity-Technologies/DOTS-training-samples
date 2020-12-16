using Unity.Burst.Intrinsics;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TillGroundSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();
        
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        var pathBuffers = GetBufferFromEntity<PathNode>();

        var random = new Unity.Mathematics.Random(1234);

        Entities
           .WithAll<Farmer>()
           .WithAll<TillGround>()
           .WithNone<TillArea>()
           .ForEach((Entity entity, in Translation translation) =>
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

                       if(state != ETileState.Empty && state != ETileState.Tilled)
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
                   ecb.AddComponent(entity, new TillArea
                   {
                       Position = new int2(minX, minY),
                       Size = new int2(width, height)
                   });
               }
               else
               {
                   if (random.NextFloat() < 0.2f)
                   {
                       ecb.RemoveComponent<TillGround>(entity);
                   }
               }

           }).Run();

        Entities
           .WithAll<Farmer>()
           .ForEach((Entity entity, in TillArea tillArea, in Translation translation) =>
           {
               var pathBuffer = pathBuffers[entity];
               var maxX = tillArea.Position.x + tillArea.Size.x;
               var maxY = tillArea.Position.y + tillArea.Size.y;

               Debug.DrawLine(new Vector3(tillArea.Position.x, .1f, tillArea.Position.y), new Vector3(maxX + 1f, .1f, tillArea.Position.y), Color.green);
               Debug.DrawLine(new Vector3(maxX + 1f, .1f, tillArea.Position.y), new Vector3(maxX + 1f, .1f, maxY + 1f), Color.green);
               Debug.DrawLine(new Vector3(maxX + 1f, .1f, maxY + 1f), new Vector3(tillArea.Position.x, .1f, maxY + 1f), Color.green);
               Debug.DrawLine(new Vector3(tillArea.Position.x, .1f, maxY + 1f), new Vector3(tillArea.Position.x, .1f, tillArea.Position.y), Color.green);

               var farmerLinearIndex = Mathf.FloorToInt(translation.Value.x) + Mathf.FloorToInt(translation.Value.y) * settings.GridSize.x;
               if(tileBuffer[farmerLinearIndex].Value == ETileState.Empty)
               {
                   tileBuffer[farmerLinearIndex] = new TileState { Value = ETileState.Tilled };
                   //ecb.RemoveComponent(entity, typeof(Path));
                   // todo till the farms ground
                   // Farm.TillGround(tileX, tileY);

               }
               else
               {
                   /*if (path.Index == pathBuffer.Length)
                   {
                       // TODO : Search for tillable tile in the tiling zone
                       var nextTile = -1; // Pathing.SearchForOne(tileX, tileY, 25, Pathing.IsNavigableDefault, Pathing.IsTillable, tillingZone);
                       if(nextTile != -1)
                       {
                           int2 tilePosition = new int2(nextTile % settings.GridSize.x, nextTile / settings.GridSize.x);
                           path.Index = 0;
                           // new path
                       }
                       else
                       {
                           ecb.RemoveComponent<TillArea>(entity);
                           ecb.RemoveComponent<TillGround>(entity);
                       }
                   }*/
               }

           }).Run();

        ecb.Playback(EntityManager);
    }
}
