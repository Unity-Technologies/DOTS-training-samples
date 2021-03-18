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
            var blob = this.GetSingleton<MetroBlobContainer>();

            Entities.WithoutBurst().ForEach((Entity entity, in PlatformQueueSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                ref BlobArray<PlatformBlob> platforms = ref blob.Blob.Value.Platforms;

                for (int i = 0; i < platforms.Length; ++i)
                {
                    var queue = ecb.CreateEntity();
                    ecb.AddComponent(queue, new PlatformQueue
                    {
                        platformIndex = i,
                    });
                    ecb.AddComponent<FirstQueuePassenger>(queue, new FirstQueuePassenger(){ passenger = Entity.Null});
                    
                    float3 queuePoint = blob.Blob.Value.Platforms[i].queuePoint;
                    
                    
                    ecb.AddComponent(queue, new Translation()
                    {
                        Value = queuePoint
                    });
                    ecb.AddBuffer<CommuterQueueData>(queue);
                }

            }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}