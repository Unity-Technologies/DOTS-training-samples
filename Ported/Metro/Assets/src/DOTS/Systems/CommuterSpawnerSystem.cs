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

                    ecb.AddBuffer<PathData>(commuter);
                    ecb.AppendToBuffer(commuter, new PathData
                    {
                        point = from.walkway_BACK_CROSS.nav_START.transform.position
                    });
                    ecb.AppendToBuffer(commuter, new PathData
                    {
                        point = from.walkway_BACK_CROSS.nav_END.transform.position
                    });
                    ecb.AppendToBuffer(commuter, new PathData
                    {
                        point = to.walkway_FRONT_CROSS.nav_END.transform.position
                    });
                    ecb.AppendToBuffer(commuter, new PathData
                    {
                        point = to.walkway_FRONT_CROSS.nav_START.transform.position
                    });
                    ecb.AddComponent<SwitchingPlatformTag>(commuter);
                    ecb.AddComponent(commuter, new CurrentPathTarget
                    {
                        currentIndex = 0,
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