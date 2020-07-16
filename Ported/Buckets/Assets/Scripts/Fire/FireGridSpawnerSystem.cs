using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Fire
{
    [GenerateAuthoringComponent]
    public struct FireGridSpawner : IComponentData
    {
        public Entity Prefab;

        public int RandomSeed;

        public int CountX;
        public int CountZ;
        public float3 Origin;

        public int MinInitialFires;
        public int MaxInitialFires;

        public float StartFireAmount;
        public float StartFireVelocity;
    }

    public struct Initialized : IComponentData { };

    public class FireGridSpawnerSystem : SystemBase
    {
        private EntityQuery UninitializedSpawners;

        struct SpawnerInfo
        {
            public Entity Prefab;

            public int RandomSeed;

            public int CountX;
            public int CountZ;
            public int TotalCount;
            public float3 Center;

            public int MinInitialFires;
            public int MaxInitialFires;

            public float StartFireAmount;
            public float StartFireVelocity;
        }

        protected override void OnCreate()
        {
            RequireForUpdate(UninitializedSpawners);
        }

        protected override void OnUpdate()
        {
            // Check if we have initialized
            EntityQuery queryGroup = GetEntityQuery(typeof(Initialized));
            if (queryGroup.CalculateEntityCount() > 0)
            {
                return;
            }

            var spawners = new NativeArray<SpawnerInfo>(UninitializedSpawners.CalculateEntityCount(), Allocator.TempJob);

            Entities.WithStoreEntityQueryInField(ref UninitializedSpawners).WithNone<Initialized>()
                .ForEach((int entityInQueryIndex, ref FireGridSpawner spawner) =>
                {
                    spawners[entityInQueryIndex] = new SpawnerInfo
                    {
                        Prefab = spawner.Prefab,
                        RandomSeed = spawner.RandomSeed,
                        CountX = spawner.CountX,
                        CountZ = spawner.CountZ,
                        TotalCount = spawner.CountX * spawner.CountZ,
                        Center = spawner.Origin,
                        StartFireAmount = spawner.StartFireAmount,
                        StartFireVelocity = spawner.StartFireVelocity,
                        MinInitialFires = spawner.MinInitialFires,
                        MaxInitialFires = spawner.MaxInitialFires
                    };
                }).Run();

            foreach (var spawner in spawners)
            {
                EntityManager.Instantiate(spawner.Prefab, spawner.TotalCount, Allocator.Temp);
            }

            EntityManager.AddComponent<Initialized>(UninitializedSpawners);

            var fireBufferEntity = GetSingletonEntity<FireBuffer>();

            SpawnerInfo spanwerInfoInstance = spawners[0];
            EntityManager.AddComponentData(fireBufferEntity, new FireBufferMetaData
            {
                CountX = spanwerInfoInstance.CountX,
                CountZ = spanwerInfoInstance.CountZ,
                TotalSize = spanwerInfoInstance.TotalCount
            });

            UnityEngine.Debug.Log($"FireGridSpawner initializing grid [{spanwerInfoInstance.CountX}, {spanwerInfoInstance.CountZ}] Total: {spanwerInfoInstance.TotalCount}");

            // Grab Fire buffer
            var gridBufferLookup = GetBufferFromEntity<FireBufferElement>();
            var gridBuffer = gridBufferLookup[fireBufferEntity];
            gridBuffer.ResizeUninitialized(spanwerInfoInstance.TotalCount);
            var gridArray = gridBuffer.AsNativeArray();

            Random random = new Random((uint)spanwerInfoInstance.RandomSeed);

            // Start initial fires
            int numberOfFires = random.NextInt(spanwerInfoInstance.MinInitialFires, spanwerInfoInstance.MaxInitialFires);
            NativeArray<int> fireIndiciesArr = new NativeArray<int>(numberOfFires, Allocator.TempJob);

            // Pick indicies of fires that should be started, at random
            for (int i = 0; i < numberOfFires; i++)
            {
                int fireIndex = random.NextInt(0, spanwerInfoInstance.TotalCount - 1);
                fireIndiciesArr[i] = fireIndex;
            }

            Entities
                .WithDeallocateOnJobCompletion(spawners)
                .WithDeallocateOnJobCompletion(fireIndiciesArr)
                .ForEach((Entity fireEntity, int entityInQueryIndex, ref Translation translation,
                    ref TemperatureComponent temperature, ref StartHeight height, in BoundsComponent bounds) =>
                {
                    for (int i = 0; i < spawners.Length; ++i)
                    {
                        var spawner = spawners[i];
                        if (entityInQueryIndex < spawner.TotalCount)
                        {
                            int x = entityInQueryIndex % spawner.CountX;
                            int z = entityInQueryIndex / spawner.CountX;

                            // Start pos
                            var posX = spawner.Center.x + bounds.SizeXZ * (x - (spawner.CountX - 1) / 2);
                            var posZ = spawner.Center.z + bounds.SizeXZ * (z - (spawner.CountZ - 1) / 2);

                            // Add random offset on y to debug that the grid spacing is correct
                            var posY = random.NextFloat(-0.01f, 0.01f) + spawner.Center.y;

                            height.Value = posY;
                            height.Variance = random.NextFloat(0.055f, 0.065f);

                            translation.Value = new float3(posX, posY, posZ);
                            
                            // If we should start a fire, start it
                            if (fireIndiciesArr.Contains(entityInQueryIndex))
                            {
                                temperature.Value = spawner.StartFireAmount;
                                temperature.Velocity = spawner.StartFireVelocity;
                            }

                            gridArray[entityInQueryIndex] = new FireBufferElement {FireEntity = fireEntity};
                            break;
                        }
                        entityInQueryIndex -= spawner.TotalCount;
                    }
                }).ScheduleParallel();
        }

        /*
        void StartFire(Entity fireEntity, float startFireAmount, float startFireVelocity)
        {
            // Set Starting fire amount
            var amount = math.clamp(startFireAmount, 0f, 1f);
            EntityManager.SetComponentData(fireEntity, new TemperatureComponent {Value = amount });

            // Set Starting fire velocity
            var velocity = math.clamp(startFireVelocity, 0f, 1f);
            EntityManager.AddComponentData(fireEntity, new TemperatureVelocity { Value = velocity });
        }
         */
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
