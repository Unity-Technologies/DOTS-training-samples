using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(TrainSpawner))]
[BurstCompile]
partial struct CommuterSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;
    Unity.Mathematics.Random Random;
    private ComponentLookup<LocalTransform> worldTransforms;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();
        Random = Unity.Mathematics.Random.CreateFromIndex(1234);
        worldTransforms = state.GetComponentLookup<LocalTransform>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var hue = Random.NextFloat();
        worldTransforms.Update(ref state);

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var commuters = state.EntityManager.Instantiate(config.CommuterPrefab, config.CommuterCount, Allocator.Temp);
        NativeList<Entity> platforms = new NativeList<Entity>(Allocator.Temp);
        //NativeList<Platform> platformsComponent = new NativeList<Platform>(Allocator.Persistent);
        foreach (var (platform, platformEntity) in SystemAPI.Query<Platform>().WithEntityAccess())
        {
            platforms.Add(platformEntity);
        }
        int platformsSize = platforms.Length;
        SpawnCommuters(state, hue, commuters, platforms, platformsSize, worldTransforms, config);

        state.Enabled = false;

    }

    public void SpawnCommuters(SystemState state, float hue, NativeArray<Entity> commuters, NativeList<Entity> platforms, int platformsSize, ComponentLookup<LocalTransform> worldTransforms, Config config)
    {
        foreach (var commuter in commuters)
        {
            int randomIndex= Random.NextInt(0, platformsSize);
            
            

            URPMaterialPropertyBaseColor RandomColor()
            {
                hue = (hue + 0.618034005f) % 1;
                var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
                return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
            }

            SystemAPI.SetComponent(commuter, RandomColor());
            
            var platformEntity = platforms[randomIndex];
            Debug.Log("Ps: " + platformEntity.ToString());

            var commuterComponent = state.EntityManager.GetComponentData<Commuter>(commuter);
            commuterComponent.CurrentPlatform = platformEntity;
            commuterComponent.Random = Random.CreateFromIndex((uint)Random.NextInt());
            state.EntityManager.SetComponentData<Commuter>(commuter, commuterComponent);
            ////state.EntityManager.SetComponentData<Commuter>(commuter, new Commuter { CurrentPlatform = platformEntity });

            var platform = state.EntityManager.GetComponentData<Platform>(platformEntity);
            //Line line = state.EntityManager.GetComponentData<Line>(platform.Line);
            //var platformTransform = state.EntityManager.GetComponentData<WorldTransform>(platform.PlatformFloor);
            //var platformFloorAspect = SystemAPI.GetAspectRW<TransformAspect>(platform.PlatformFloor);

            var commuterTransforAspect = SystemAPI.GetAspectRW<TransformAspect>(commuter);
            //var platformTransfromAspect = SystemAPI.GetAspectRW<TransformAspect>(platform.PlatformFloor);

            var station = state.EntityManager.GetComponentData<Parent>(platformEntity);
            //var stationTransformAspect = SystemAPI.GetAspectRW<TransformAspect>(station.Value);
            var stationComponent = state.EntityManager.GetComponentData<Station>(station.Value);
            var stationTransform = state.EntityManager.GetComponentData<LocalTransform>(station.Value);
            //var stationTransform = stationTransformAspect.WorldPosition;

            var randomOffsetXMax = config.StationsOffset * stationComponent.Id;
            var randomOffsetZMax = stationTransform.Position.z + (platform.Id * -50f);
            var randomOffsetX = Random.NextFloat(randomOffsetXMax - 30f, randomOffsetXMax + 30f); ;
            var randomOffsetZ = Random.NextFloat(randomOffsetZMax - 5f, randomOffsetZMax + 5f); ;

            var randomOffset = new float3(randomOffsetX, 0, randomOffsetZ);


            commuterTransforAspect.WorldPosition = ( randomOffset);
            ////commuterTransforAspect.TranslateWorld (stationTransform + commuterTransforAspect.WorldPosition + randomOffet);

            //state.EntityManager.SetComponentData<WorldTransform>(commuter, new WorldTransform { Position = randomOffset, Scale = 1 });

        }
    }
}
