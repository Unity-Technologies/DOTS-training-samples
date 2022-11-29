using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


[BurstCompile]
partial struct SpawnerSystem : ISystem
{
    [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformFromEntity;
    EntityQuery m_BaseColorQuery;
    private Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Random.CreateFromIndex(1234);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var config = SystemAPI.GetSingleton<Config>();
        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
     

        for (int i = 0; i < config.PlatformCountPerStation; i++)
        {
            LocalTransform spawnLocalToWorld = LocalTransform.FromPosition(9 * i, 0, 0);

            SpawnPlatform(ref state, ecb, spawnLocalToWorld, config);
            SpawnRail(ref state, ecb, spawnLocalToWorld, config);
            SpawnTrain(ref state, ecb, spawnLocalToWorld, config);
            for(int c = 0; c < 10; c++)
            {
                LocalTransform personSpawn = LocalTransform.FromPosition(2+9 * i, -0.1f, -22+2*c);
                SpawnPerson(ref state, ecb, personSpawn, config);
            }
        }

        state.Enabled = false;
    }

    private void SpawnTrain(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Config config)
    {
        Entity train = state.EntityManager.Instantiate(config.TrainPrefab);
        ecb.SetComponent<LocalTransform>(train, spawnLocalToWorld);
    }

    private void SpawnRail(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Config config)
    {
        Entity rail = ecb.Instantiate(config.RailsPrefab);
        ecb.SetComponent<LocalTransform>(rail, spawnLocalToWorld);
    }

    private void SpawnPlatform(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Config config)
    {
        Entity platform = state.EntityManager.Instantiate(config.PlatformPrefab);
        ecb.SetComponent<LocalTransform>(platform, spawnLocalToWorld);
    }

    private void SpawnPerson(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Config config)
    {
        var hue = random.NextFloat();
        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        Entity person = state.EntityManager.Instantiate(config.PersonPrefab);
        ecb.SetComponent<LocalTransform>(person, spawnLocalToWorld);
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();
        ecb.SetComponentForLinkedEntityGroup(person, queryMask, RandomColor());
    }
}