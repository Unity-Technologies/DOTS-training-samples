using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.Rendering;
using Unity.VisualScripting;

[BurstCompile]
public partial struct SpawningSystem : ISystem
{
    Unity.Mathematics.Random random;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex(1234);
        state.RequireForUpdate<Config>();
        onlyOnce = true;
        wallSpawn = true;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    bool onlyOnce;
    bool wallSpawn;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //WHY DO WE NEED TO DO THIS
        //ASk Brian
        //Querying for singleton config gives 2 configs.
        if (!onlyOnce) return;

        random = Unity.Mathematics.Random.CreateFromIndex(1234);
        var hue = random.NextFloat();
        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var config = SystemAPI.GetSingleton<Config>();

        //foreach(var config in )SystemAPI.Query<RefRO<Config>>();

        float ballSpread = 50;
        float wallSpread = 50;
        //for (int i = 0; i < config.BallCount; i++)
        //{
        //    float3 displacement = random.NextFloat3(new float3(-1, 0, -1) * ballSpread, new float3(1, 0, 1) * ballSpread);

        //}

        //TODO: Jobify the creation of the map
        //TODO: Profile to see if instantiation is jobified.
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        //var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        //Need to access the native array instead of Query after you create stuff
        //Ask brian
        var balls = CollectionHelper.CreateNativeArray<Entity>(config.BallCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.BallPrefab, balls);

        

        state.EntityManager.AddComponent<LocalTransform>(SystemAPI.QueryBuilder().WithAll<Ball>());
        
        //Need to access the native array instead of Query after you create stuff
        //Ask brian
        var walls = CollectionHelper.CreateNativeArray<Entity>(config.ObstacleCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.ObstaclePrefab, walls);

        ecb.Playback(state.EntityManager);


        //state.EntityManager.Instantiate()
        

        NativeArray<float3> wallPositions = new NativeArray<float3>(config.ObstacleCount, Allocator.Temp);
        

        int i = 0;
        foreach (var wall in walls)
        {
            float3 displacement = random.NextFloat3(new float3(-1, 0, -1) * wallSpread, new float3(1, 0, 1) * wallSpread);
            ecb.SetComponent<LocalTransform>(wall, LocalTransform.FromPosition(displacement));
            
            wallPositions[i] = displacement;
            i++;
        }

        

        foreach (var ball in balls)
        {
            float3 displacement = random.NextFloat3(new float3(-1, 0, -1) * ballSpread, new float3(1, 0, 1) * ballSpread);
            bool ballFoundValidSpot = false;
            ecb.SetComponent(ball, new URPMaterialPropertyBaseColor { Value = RandomColor().Value });
            while (!ballFoundValidSpot)
            {
                ballFoundValidSpot = true;
                int wallI = 0;
                foreach (var wall in walls)
                {
                    //var localComponentLookup = state.GetComponentLookup<LocalTransform>(false);
                    //var wallPos = localComponentLookup[wall];

                    var localTrans = wallPositions[wallI];
                    var diff = localTrans - displacement;
                    if (math.lengthsq(diff) < 2.0f)
                    {
                        ballFoundValidSpot = false;
                        displacement = random.NextFloat3(new float3(-1, 0, -1) * ballSpread, new float3(1, 0, 1) * ballSpread);
                        break;
                    }
                    wallI++;
                }
                
                ecb.SetComponent<LocalTransform>(ball, LocalTransform.FromPositionRotationScale(displacement, Quaternion.identity, 0.5f));
            }
            
            
        }

        ecb.Dispose();
        onlyOnce = false;
    }
}
