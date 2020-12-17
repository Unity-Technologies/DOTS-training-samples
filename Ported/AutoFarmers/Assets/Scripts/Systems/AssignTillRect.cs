using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FarmerIntentionSystem))]
public class AssignTillRect : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();

        var tileBuffer = GetBufferFromEntity<TileState>()[data];

        var random = new Unity.Mathematics.Random(1234);

        Entities
           .WithAll<Farmer>()
           .WithAll<TillGroundIntention>()
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
                   ecb.AddComponent(entity, new TillArea
                   {
                       Rect = new RectInt(minX, minY, width, height),
                   });
               }
               else
               {
                   if (random.NextFloat() < 0.2f)
                   {
                       ecb.RemoveComponent<TillGroundIntention>(entity);
                   }
               }

           }).Run();

        ecb.Playback(EntityManager);
    }
}
