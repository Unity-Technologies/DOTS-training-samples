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
        if (!SystemAPI.HasSingleton<Config>()) return;

        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var config = SystemAPI.GetSingleton<Config>();
        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());


        for (int i = 0; i < config.PlatformCountPerStation; i++)
        {
            LocalTransform railTransform = LocalTransform.FromPosition(9 * i, 0, 0);
            SpawnRail(ref state, ecb, railTransform, config.RailsPrefab);
            SpawnTrain(ref state, ecb, railTransform, config.TrainPrefab, config);

            for (int n = 0; n < config.NumberOfStations; n++)
            {
                LocalTransform spawnLocalToWorld = LocalTransform.FromPosition(9 * i, 0, -Globals.RailSize*0.5f+(Globals.RailSize / (config.NumberOfStations+1)) * (n+1));
                SpawnPlatform(ref state, ecb, spawnLocalToWorld, config.PlatformPrefab);

                for (int c = 0; c < 10; c++)
                {
                    LocalTransform personSpawn = LocalTransform.FromPosition(2 + spawnLocalToWorld.Position.x, spawnLocalToWorld.Position.y- 0.1f, spawnLocalToWorld.Position.z - 22 + 2 * c);
                    SpawnPerson(ref state, ecb, personSpawn, config.PersonPrefab);
                }
            }
        }

        state.Enabled = false;
    }

    private void SpawnTrain(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Entity prefab, Config config)
    {
        LocalTransform trainTransform = LocalTransform.FromPosition(spawnLocalToWorld.Position.x, spawnLocalToWorld.Position.y, random.NextInt(-(int)Globals.RailSize/2, (int)Globals.RailSize / 2));
        Entity train = state.EntityManager.Instantiate(prefab);
        ecb.SetComponent<LocalTransform>(train, trainTransform);
        Waypoint waypoint = new Waypoint();
        float pos = Globals.RailSize * 0.5f + trainTransform.Position.z;
        waypoint.WaypointID = (int)(pos / (Globals.RailSize / (config.NumberOfStations + 1)));
        ecb.SetComponent(train, waypoint);
        IdleTime idleTime = new IdleTime();
        idleTime.Value = 0f;
        ecb.SetComponent(train, idleTime);
    }

    private void SpawnRail(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Entity prefab)
    {
        Entity rail = ecb.Instantiate(prefab);
        ecb.SetComponent<LocalTransform>(rail, spawnLocalToWorld);
    }

    private void SpawnPlatform(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Entity prefab)
    {
        Entity platform = state.EntityManager.Instantiate(prefab);
        ecb.SetComponent<LocalTransform>(platform, spawnLocalToWorld);
    }

    private void SpawnPerson(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Entity prefab)
    {
        var hue = random.NextFloat();
        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        Entity person = state.EntityManager.Instantiate(prefab);
        ecb.SetComponent<LocalTransform>(person, spawnLocalToWorld);
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();
        ecb.SetComponentForLinkedEntityGroup(person, queryMask, RandomColor());
    }
}