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
        var pathMovement = World.GetExistingSystem<PathMovement>();
        var random = new Unity.Mathematics.Random(1234);


        Entities
           .WithAll<Farmer>()
           .ForEach((Entity entity, ref TillGroundIntention tillGround, in Translation translation) =>
           {
               var tillingZone = tillGround.Rect;
               if (tillingZone.Equals(default))
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
                       tillGround.Rect = new RectInt(minX, minY, width, height);
                   }
                   else
                   {
                       if (random.NextFloat() < 0.2f)
                       {
                           ecb.RemoveComponent<TillGroundIntention>(entity);
                       }
                   }
               }
               else
               {
                   var pathBuffer = pathBuffers[entity];
                   Debug.DrawLine(new Vector3(tillingZone.min.x, .1f, tillingZone.min.y), new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.min.y), Color.green);
                   Debug.DrawLine(new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.min.y), new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.max.y + 1f), Color.green);
                   Debug.DrawLine(new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.max.y + 1f), new Vector3(tillingZone.min.x, .1f, tillingZone.max.y + 1f), Color.green);
                   Debug.DrawLine(new Vector3(tillingZone.min.x, .1f, tillingZone.max.y + 1f), new Vector3(tillingZone.min.x, .1f, tillingZone.min.y), Color.green);

                   var farmerPosition = new int2(Mathf.FloorToInt(translation.Value.x), Mathf.FloorToInt(translation.Value.z));
                   var farmerLinearIndex = farmerPosition.x + farmerPosition.y * settings.GridSize.x;

                   bool isTillable = false;
                   if (tileBuffer[farmerLinearIndex].Value == ETileState.Empty)
                   {
                       if (farmerPosition.x >= tillingZone.xMin && farmerPosition.x <= tillingZone.xMax)
                       {
                           if (farmerPosition.y >= tillingZone.yMin && farmerPosition.y <= tillingZone.yMax)
                           {
                               isTillable = true;
                           }
                       }
                   }

                   if (isTillable)
                   {
                       pathBuffer.Clear();
                       tileBuffer[farmerLinearIndex] = new TileState { Value = ETileState.Tilled };
                   }
                   else
                   {
                       if (pathBuffer.Length == 0)
                       {
                           int tileHash = pathMovement.SearchForOne(farmerPosition.x, farmerPosition.y, 600, tileBuffer, pathMovement.defaultNavigation, pathMovement.isTillable, tillingZone);
                           if (tileHash != -1)
                           {
                               int tileX, tileY;
                               pathMovement.Unhash(tileHash, out tileX, out tileY);
                               pathMovement.AssignLatestPath(pathBuffer, tileX, tileY);
                           }
                           else
                           {
                               ecb.RemoveComponent<TillGroundIntention>(entity);
                           }
                       }
                   }
               }
            

           }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
    }
}
