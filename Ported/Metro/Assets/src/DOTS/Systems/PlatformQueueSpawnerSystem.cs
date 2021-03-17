using src.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace src.DOTS.Systems
{
    public class PlatformQueueSpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var metro = this.GetSingleton<GameObjectRefs>();

            Entities.WithoutBurst().ForEach((Entity entity, in PlatformQueueSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                Platform[] platforms = metro.metro.allPlatforms;

                for (int i = 0; i < platforms.Length; ++i)
                {
                    var queue = ecb.CreateEntity();
                    ecb.AddComponent<PlatformQueue>(queue, new PlatformQueue()
                    {
                        platformIndex = i
                    });
                    ecb.AddBuffer<CommuterQueueData>(queue);
                }

            }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}