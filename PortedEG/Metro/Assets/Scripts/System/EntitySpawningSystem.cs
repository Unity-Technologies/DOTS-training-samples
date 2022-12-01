using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;
using Unity.Burst;

public partial class EntitySpawningSystem : SystemBase
{
    private bool IsSpawningDone = false;

    EntityQuery m_BaseColorQuery;

    protected override void OnCreate()
    {
        m_BaseColorQuery = GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
    }

    [BurstCompile]
    public void Spawn()
    {
        if (IsSpawningDone)
            return;

        Metro metro = Metro.INSTANCE;

        var config = SystemAPI.GetSingleton<Config>();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        // Platform
        {
            int objCount = metro.allPlatforms.Length;

            var platformSpawnJob = new PlatfromSpawnJob
            {
                entity = config.PlatformPrefab,
                Ecb = ecb.AsParallelWriter(),
                Count = objCount
            };

            var spawnHandle = platformSpawnJob.Schedule(objCount, 128);
            spawnHandle.Complete();
        }

        // Commuter
        {
            int objCount = metro.commuters.Count;

            var commuterSpawnJob = new CommuterSpawnJob
            {
                entity = config.CommuterPrefab,
                Ecb = ecb.AsParallelWriter(),
                Count = objCount,
                mask = m_BaseColorQuery.GetEntityQueryMask()
            };

            var spawnHandle = commuterSpawnJob.Schedule(objCount, 128);
            spawnHandle.Complete();
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();

        IsSpawningDone = true;
    }

    protected override void OnUpdate()
    {
        Spawn();

        Metro.INSTANCE.IS_COMMUTER_SPAWNED = IsSpawningDone;
    }

    [HideInInspector] private Platform[] m_AllPlatforms;
    [HideInInspector] private List<Commuter> m_AllCommuters;

    private bool m_Spawned = false;

    // Example Burst job that creates many entities
    [GenerateTestsForBurstCompatibility]
    public struct PlatfromSpawnJob : IJobParallelFor
    {
        public Entity entity;
        public int Count;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute(int index)
        {
            // Clone the Prototype entity to create a new entity.
            var e = Ecb.Instantiate(index, entity);

        }
    }


    [BurstCompile]
    public struct CommuterSpawnJob : IJobParallelFor
    {
        public Entity entity;
        public int Count;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityQueryMask mask;

        [BurstCompile]
        public void Execute(int index)
        {
            // Clone the Prototype entity to create a new entity.
            var e = Ecb.Instantiate(index, entity);

            // Prototype has all correct components up front, can use SetComponent to
            // set values unique to the newly created entity, such as the transform.
            //int matIndex = singleMat ? 0 : index;
            //Ecb.SetComponent(index, e, MaterialMeshInfo.FromRenderMeshArrayIndices(matIndex, 0));
            //Ecb.SetComponent(index, e, new LocalToWorld { Value = ComputeTransform(index) });


            // This system will only run once, so the random seed can be hard-coded.
            // Using an arbitrary constant seed makes the behavior deterministic.
            var random = Unity.Mathematics.Random.CreateFromIndex((uint)index);
            var hue = random.NextFloat();

            // Helper to create any amount of colors as distinct from each other as possible.
            // The logic behind this approach is detailed at the following address:
            // https://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
            URPMaterialPropertyBaseColor RandomColor()
            {
                // Note: if you are not familiar with this concept, this is a "local function".
                // You can search for that term on the internet for more information.

                // 0.618034005f == 2 / (math.sqrt(5) + 1) == inverse of the golden ratio
                hue = (hue + 0.618034005f) % 1;
                var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
                return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
            }


            CommuterSpeed RandomSpeed()
            {
                float3 speed = new float3(random.NextFloat(-1f, 1f), 0f, random.NextFloat(-1f, 1f));
                return new CommuterSpeed { Value = speed };
            }

            Ecb.SetComponentForLinkedEntityGroup(index, e, mask, RandomColor());

            Ecb.SetComponent(index, e, RandomSpeed());

            float4x4 transform = float4x4.TRS(new float3(10, 0, 0), quaternion.identity, 5.5f);
            Ecb.SetComponent(index, e, new LocalTransform { Position = new float3(10, 0, 0), Scale = 1.5f, Rotation = quaternion.identity }); ;
        }
    }
}