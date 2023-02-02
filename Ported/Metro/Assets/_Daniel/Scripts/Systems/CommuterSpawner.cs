using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(StationSpawner))]
[BurstCompile]
partial struct CommuterSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;
    Random Random;
    ComponentLookup<WorldTransform> m_WorldTransformLookup;


    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();
        Random = Unity.Mathematics.Random.CreateFromIndex(1234);
        m_WorldTransformLookup = state.GetComponentLookup<WorldTransform>(true);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var hue = Random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4) color };
        }

        var commuters = state.EntityManager.Instantiate(config.CommuterPrefab, config.CommuterCount, Allocator.Persistent);
        NativeList<Entity> platforms = new NativeList<Entity>(Allocator.Temp);
        foreach (var (platform, platformEntity) in SystemAPI.Query<Platform>().WithEntityAccess())
        {
            platforms.Add(platformEntity);
        }
        int platformsSize = platforms.Length;
        foreach (var commuter in commuters)
        {
            
            var position1 = new float3();
            position1.xz = Random.NextFloat2() * 4;

            var platform = platforms[Random.NextInt(0, platformsSize)];

            state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(commuter, RandomColor());
            //state.EntityManager.SetComponentData<Platform>(commuter, platform);
            var commuterComponent = state.EntityManager.GetComponentData<Commuter>(commuter);
            commuterComponent.CurrentPlatform = platform;
            state.EntityManager.SetComponentData<Commuter>(commuter, commuterComponent);

            Platform p = state.EntityManager.GetComponentData<Platform>(platform);
            //TransformAspect platformTransform = state.EntityManager.GetAspect<TransformAspect>(p.PlatformFloor);

            //WorldTransform platformTransform = state.EntityManager.GetComponentData<WorldTransform>(p.PlatformFloor);
            //float randomX = platformTransform.Position.x + 5 + Random.NextFloat(-platformTransform.Scale, platformTransform.Scale) * 5;
            //float randomZ = platformTransform.Position.z + 5 + Random.NextFloat(-platformTransform.Scale, platformTransform.Scale) * 5;

            var platformTransform = state.EntityManager.GetComponentData<WorldTransform>(p.PlatformFloor);
            //var pa = SystemAPI.GetComponent<ParentTransform>(p.PlatformFloor);
            var aspect = SystemAPI.GetAspectRW<TransformAspect>(p.PlatformFloor);

            var commuterTransforAspect = SystemAPI.GetAspectRW<TransformAspect>(commuter);

            commuterTransforAspect.TranslateWorld(aspect.TransformPointLocalToWorld(platformTransform.Position) + Random.NextFloat3(-5, 5));
            //float randomX = as.Position.x  + Random.NextFloat(-5f, 5f);
            //float randomZ = platformTransform.Position.z + Random.NextFloat(-5f, 5f);
            //float randomX = platformTransform.Position.x  + Random.NextFloat(-5f, 5f);
            //float randomZ = platformTransform.Position.z + Random.NextFloat(-5f, 5f);

            /*var position = new float3(randomX, 0.5f, randomZ);
            //var position = new float3(Random.nex * 2, 0.5f, c*2);
            //var commuterTransform = SystemAPI.GetComponent<LocalToWorld>(comm);
            var commuterTransform = SystemAPI.GetComponent<LocalTransform>(commuter);
            //commuterTransform.Position += new float3(randomX, 1.5f, randomZ);
            //state.EntityManager.SetComponentData<LocalTransform>(commuter, new LocalTransform { Position = position1, Scale = 1 });
            commuterTransform.Position = position;
            state.EntityManager.SetComponentData<LocalTransform>(commuter, commuterTransform);
       */
                }

        state.Enabled = false;
    }
}
