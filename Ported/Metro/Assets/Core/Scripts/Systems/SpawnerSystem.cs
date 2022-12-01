using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;


[BurstCompile]
partial struct SpawnerSystem : ISystem
{
    [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformFromEntity;
    EntityQuery m_BaseColorQuery;
    private Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

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
        var configEntity = SystemAPI.GetSingletonEntity<Config>();
        DynamicBuffer<TrainPositionsBuffer> trainPositions = state.EntityManager.AddBuffer<TrainPositionsBuffer>(configEntity);
        
        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
        trainPositions.Length = config.PlatformCountPerStation;

        for (int i = 0; i < config.PlatformCountPerStation; i++)
        {
            LocalTransform railTransform = LocalTransform.FromPosition(Globals.PlatformSpacing * i, 0, 0);
            SpawnRail(ref state, ecb, railTransform, config.RailsPrefab);
            SpawnTrain(ref state, ecb, railTransform, config.TrainPrefab, config, i);

            for (int n = 0; n < config.NumberOfStations; n++)
            {
                var platformID = i * config.NumberOfStations + n;
                
                LocalTransform spawnLocalToWorld = LocalTransform.FromPosition(Globals.PlatformSpacing * i, 0, -Globals.RailSize*0.5f+(Globals.RailSize / (config.NumberOfStations+1)) * (n+1));
                SpawnPlatform(ref state, ecb, spawnLocalToWorld, config.PlatformPrefab);
                var path = SpawnPath(ref state, ecb, spawnLocalToWorld, config.PathPrefab, platformID);
                var pathCom = SystemAPI.GetComponent<Path>(path);
                var defaultWaypoint = pathCom.Default;
                var initTarget = spawnLocalToWorld.Position;

                var hue = random.NextFloat();
                for (int c = 0; c < config.PersonCount; c++)
                {
                    var personSpawn = WorldTransform.FromPosition(5 + spawnLocalToWorld.Position.x, spawnLocalToWorld.Position.y- 0.1f, spawnLocalToWorld.Position.z - 22 + random.NextFloat(10));
                    SpawnPerson(ref state, ecb, personSpawn, config.PersonPrefab, platformID, defaultWaypoint, initTarget, hue);
                }
            }
        }
        
        state.Enabled = false;
    }

    private void SpawnTrain(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Entity prefab, Config config, int index)
    {
        LocalTransform trainTransform = LocalTransform.FromPosition(spawnLocalToWorld.Position.x, spawnLocalToWorld.Position.y, random.NextInt(-(int)Globals.RailSize/2, (int)Globals.RailSize / 2));
        Entity train = state.EntityManager.Instantiate(prefab);
        ecb.SetComponent(train, trainTransform);
        
        WaypointID waypoint = new WaypointID();
        float pos = Globals.RailSize * 0.5f + trainTransform.Position.z;
        waypoint.Value = (int)(pos / (Globals.RailSize / (config.NumberOfStations + 1)));
        ecb.AddComponent(train, waypoint);
        
        IdleTime idleTime = new IdleTime();
        idleTime.Value = 0f;
        ecb.SetComponent(train, idleTime);
        
        TrainInfo trainInfo = new TrainInfo();
        trainInfo.Id = index;
        ecb.SetComponent(train, trainInfo);
        
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
    
    private Entity SpawnPath(ref SystemState state, EntityCommandBuffer ecb, LocalTransform spawnLocalToWorld, Entity prefab, int index)
    {
        Entity path = state.EntityManager.Instantiate(prefab);
        ecb.SetComponent(path, spawnLocalToWorld);
        ecb.SetComponent(path, new PathID { Value = index });

        return path;
    }

    private Entity SpawnPerson(ref SystemState state, EntityCommandBuffer ecb, WorldTransform spawnLocalToWorld, Entity prefab, int platformID, Entity defaultWaypoint, float3 waypointPos, float hue)
    {
        
        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        Entity person = state.EntityManager.Instantiate(prefab);
        ecb.SetComponent(person, spawnLocalToWorld);
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();
        ecb.SetComponentForLinkedEntityGroup(person, queryMask, RandomColor());
        
        ecb.AddComponent(person, new DestinationPlatform{ Value = platformID});
        ecb.AddComponent(person, new Agent{ CurrentWaypoint = defaultWaypoint});
        ecb.AddComponent(person, new TargetPosition{ Value = waypointPos});
        ecb.AddComponent(person, new Speed { Value = 5f+random.NextFloat(3) });
        ecb.AddComponent(person, new WaypointMovementTag());
        //ecb.AddComponent(person, new PassengerTag());

        return person;
    }
}