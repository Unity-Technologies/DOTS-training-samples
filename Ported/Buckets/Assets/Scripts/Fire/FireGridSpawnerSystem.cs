using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Fire.Authoring
{
    [GenerateAuthoringComponent]
    public struct FireGridSpawner : IComponentData
    {
        public Entity Prefab;
        public int CountX;
        public int CountZ;
    }

    public struct Initialised : IComponentData { };

    public class FireGridSpawnerSystem : SystemBase
    {
        private EntityQuery UninitializedSpawners;

        struct SpawnerInfo
        {
            public Entity Prefab;
            public int CountX;
            public int CountZ;
            public int TotalCount;
            public float3 Center;
        }

        protected override void OnCreate()
        {
            RequireForUpdate(UninitializedSpawners);
        }

        protected override void OnUpdate()
        {
            var spawners = new NativeArray<SpawnerInfo>(UninitializedSpawners.CalculateEntityCount(), Allocator.TempJob);

            Entities.WithStoreEntityQueryInField(ref UninitializedSpawners).WithNone<Initialised>()
                .ForEach((int entityInQueryIndex, ref FireGridSpawner spawner, in LocalToWorld ltw) =>
                {
                    spawners[entityInQueryIndex] = new SpawnerInfo
                    {
                        Prefab = spawner.Prefab,
                        CountX = spawner.CountX,
                        CountZ = spawner.CountZ,
                        TotalCount = spawner.CountX * spawner.CountZ,
                        Center = ltw.Position
                    };
                }).Run();

            foreach (var spawner in spawners)
            {
                EntityManager.Instantiate(spawner.Prefab, spawner.TotalCount, Allocator.Temp);
            }

            EntityManager.AddComponent<Initialised>(UninitializedSpawners);

            Entities.WithDeallocateOnJobCompletion(spawners)
                .ForEach((int entityInQueryIndex, ref Translation translation, in BoundsComponent bounds) =>
                {

                    // TODO write
                    for (int i = 0; i < spawners.Length; ++i)
                    {
                        var spawner = spawners[i];
                        if (entityInQueryIndex < spawner.TotalCount)
                        {
                            int x = entityInQueryIndex % spawner.CountX;
                            int z = entityInQueryIndex / spawner.CountX;
                            var posX = bounds.Size / 2 * (x - (spawner.CountX - 1) / 2);
                            var posZ = bounds.Size / 2 * (z - (spawner.CountZ - 1) / 2);
                            translation.Value = spawner.Center + new float3(posX, 0, posZ);
                            break;
                        }
                        entityInQueryIndex -= spawner.TotalCount;
                    }
                }).ScheduleParallel();
        }
    }
}

/*
 * Entity for all fires (Grid storage)
 * On that entity we can add a dynamic buffer component (native list, attached to an entity)
 * new struct IBufferElementData (entity fire)
 *
 * As we initialize this list - make sure all the entity references are correct
 * - Can access the singleton reference
 * - Or we can pass the native array into jobs to work with
*/
