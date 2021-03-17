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
            var random = new Random(12345);

            Entities.WithoutBurst().ForEach((Entity entity, in CommuterSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                
                for (int i = 0; i < spawner.amountToSpawn; i++)
                {
                    var from = GetRandomPlatform(metro.metro, ref random);
                    var to = from.adjacentPlatforms[0];

                    var commuter = ecb.Instantiate(spawner.commuterPrefab);
                    ecb.SetComponent(commuter, new Translation {Value = from.queuePoints[0].transform.position });

                    
                    // TODO: move to switching platform system
                    ecb.AddBuffer<PathData>(commuter);
                    ecb.AddComponent<SwitchingPlatformTag>(commuter, new SwitchingPlatformTag()
                    {
                        platformFrom = from.globalIndex,
                        platformTo = to.globalIndex
                    });
                }
            }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        static Platform GetRandomPlatform(Metro metro, ref Random random)
        {
            int _PLATFORM_INDEX = (int) math.floor(random.NextFloat(0f, metro.allPlatforms.Length));
            return metro.allPlatforms[_PLATFORM_INDEX];
        }
    }
}