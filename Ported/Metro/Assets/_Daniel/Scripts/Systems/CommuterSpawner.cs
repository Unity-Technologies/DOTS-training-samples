using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(StationSpawner))]
[BurstCompile]
partial struct CommuterSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;
    Unity.Mathematics.Random Random;
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
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var commuters = state.EntityManager.Instantiate(config.CommuterPrefab, config.CommuterCount, Allocator.Persistent);
        NativeList<Entity> platforms = new NativeList<Entity>(Allocator.Temp);
        foreach (var (platform, platformEntity) in SystemAPI.Query<Platform>().WithEntityAccess())
        {
            platforms.Add(platformEntity);
        }
        int platformsSize = platforms.Length;
        Debug.Log("Platforms: " + platformsSize);
        SpawnCommuters(state, hue, commuters, platforms, platformsSize);

        state.Enabled = false;

    }

    public void SpawnCommuters(SystemState state, float hue, NativeArray<Entity> commuters, NativeList<Entity> platforms, int platformsSize)
    {
        foreach (var commuter in commuters)
        {

            var position1 = new float3();
            position1.xz = Random.NextFloat2() * 4;

            var platform = platforms[Random.NextInt(0, platformsSize)];

            var commuterComponent = state.EntityManager.GetComponentData<Commuter>(commuter);
            commuterComponent.CurrentPlatform = platform;
            state.EntityManager.SetComponentData<Commuter>(commuter, commuterComponent);

            Platform p = state.EntityManager.GetComponentData<Platform>(platform);

            var platformTransform = state.EntityManager.GetComponentData<LocalTransform>(p.PlatformFloor);

            var aspect = SystemAPI.GetAspectRW<TransformAspect>(p.PlatformFloor);

            var commuterTransforAspect = SystemAPI.GetAspectRW<TransformAspect>(commuter);

            commuterTransforAspect.TranslateWorld(aspect.TransformPointLocalToWorld(platformTransform.Position));
        }
    }
}
