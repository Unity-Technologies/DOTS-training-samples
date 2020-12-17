using Unity.Burst.Intrinsics;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(AssignTillRect))]
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

        Entities
           .WithAll<Farmer>()
           .ForEach((Entity entity, in TillArea tillArea, in Translation translation) =>
           {
               var pathBuffer = pathBuffers[entity];
               var tillingZone = tillArea.Rect;

               Debug.DrawLine(new Vector3(tillingZone.min.x, .1f, tillingZone.min.y), new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.min.y), Color.green);
               Debug.DrawLine(new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.min.y), new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.max.y + 1f), Color.green);
               Debug.DrawLine(new Vector3(tillingZone.max.x + 1f, .1f, tillingZone.max.y + 1f), new Vector3(tillingZone.min.x, .1f, tillingZone.max.y + 1f), Color.green);
               Debug.DrawLine(new Vector3(tillingZone.min.x, .1f, tillingZone.max.y + 1f), new Vector3(tillingZone.min.x, .1f, tillingZone.min.y), Color.green);

               var farmerPosition = new int2(Mathf.FloorToInt(translation.Value.x), Mathf.FloorToInt(translation.Value.y));
               var farmerLinearIndex = farmerPosition.x + farmerPosition.y * settings.GridSize.x;
               if (tileBuffer[farmerLinearIndex].Value == ETileState.Empty)
               {
                   pathBuffer.Clear();
                   tileBuffer[farmerLinearIndex] = new TileState { Value = ETileState.Tilled };
               }
               else
               {
                   if (pathBuffer.Length == 0)
                   {
                       int tileHash = pathMovement.SearchForOne(farmerPosition.x, farmerPosition.y, 25, tileBuffer, pathMovement.defaultNavigation, pathMovement.isTillable, tillingZone);
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
           }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
    }
}
