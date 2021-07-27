using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace DOTSRATS
{
    public class SpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            //Entities
            //    .ForEach((Entity entity, /*in Spawner spawner*/) =>
            //    {

            //    }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}