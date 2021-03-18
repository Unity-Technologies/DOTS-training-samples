using src.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace src.DOTS.Systems
{
    public class CommuterSpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var metro = this.GetSingleton<GameObjectRefs>();
            MetroBlobContainer blob = this.GetSingleton<MetroBlobContainer>();
            var random = new Random(12345);

            Entities.WithoutBurst().ForEach((Entity entity, in CommuterSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                
                for (int i = 0; i < spawner.amountToSpawn; i++)
                {
                    ref var valuePlatforms = ref blob.Blob.Value.Platforms;
                    var from = GetRandomPlatform(ref valuePlatforms, ref random);
                    var to = valuePlatforms[from.oppositePlatformIndex];

                    float3 p0 = from.queuePoint;
                    float3 p1 = valuePlatforms[from.oppositePlatformIndex].queuePoint;
                    float t = random.NextFloat();
                    
                    var commuter = ecb.Instantiate(spawner.commuterPrefab);
                    ecb.SetComponent(commuter, new Translation {Value =  p0 * (1.0f - t) + p1 * t });

                    // TODO: move to switching platform system
                    ecb.AddBuffer<PathData>(commuter);
                    ecb.AddComponent<SwitchingPlatformTag>(commuter);
                    ecb.AddComponent(commuter, new SwitchingPlatformData
                    {
                        platformFrom = from.platformIndex,
                        platformTo = to.platformIndex
                    });
                }
            }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        static PlatformBlob GetRandomPlatform(ref BlobArray<PlatformBlob> platforms, ref Random random)
        {
            var index = (int) math.floor(random.NextFloat(0f, platforms.Length));
            return platforms[index];
        }
    }
}