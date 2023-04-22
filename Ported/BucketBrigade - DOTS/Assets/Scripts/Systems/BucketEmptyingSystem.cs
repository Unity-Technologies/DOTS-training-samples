using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BucketMovingSystem))]
[BurstCompile]
public partial struct BucketEmptyingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    { 
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<OnFire>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float minDist;
        var config = SystemAPI.GetSingleton<Config>();
        var ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        //For each bucket with EmptyingTag enabled
        foreach (var (bucketParams, bucketTransform, bucketColor, bucket) in SystemAPI.Query<RefRW<Bucket>, RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>().WithEntityAccess().WithAll<EmptyingTag>())
        {
            minDist = float.MaxValue;
            Entity closestFire = Entity.Null;
            float3 closestFirePosition = new float3(0.0f);
            
            EntityQuery fireQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, OnFire>().Build();
            NativeArray<LocalTransform> fireTransforms = fireQ.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            NativeArray<Entity> fireE = fireQ.ToEntityArray(Allocator.Temp);
            
            for (int i = 0; i <fireTransforms.Length; i++)
            {
                var dist = Vector3.Distance(fireTransforms[i].Position, bucketTransform.ValueRO.Position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestFire = fireE[i];
                    closestFirePosition = fireTransforms[i].Position;
                }
            }


            fireTransforms.Dispose();
            fireE.Dispose();

            if (closestFire == Entity.Null) return;

            //Empty the bucket
            bucketParams.ValueRW.CurrCapacity = 0;

            //Change bucket color
            float fillFactor = bucketParams.ValueRO.CurrCapacity / bucketParams.ValueRO.MaxCapacity;
            float4 BucketColor()
            { 
                var color = Color.Lerp(config.colour_bucket_empty, config.colour_bucket_full, fillFactor);
                return (UnityEngine.Vector4)color;
            }
            bucketColor.ValueRW.Value = BucketColor();

            //change bucket size
            float scale = Mathf.Lerp(config.bucketSize_EMPTY, config.bucketSize_FULL, fillFactor);
            bucketTransform.ValueRW.Scale = scale;
            
            //Change tags on bucket
            SystemAPI.SetComponentEnabled<EmptyTag>(bucket, true);
            SystemAPI.SetComponentEnabled<FullTag>(bucket, false);
            SystemAPI.SetComponentEnabled<EmptyingTag>(bucket, false);
            
            //set the temperature of the tiles
            var fireExtuinguishJob = new FireExtuinguishJob
            {
                centerFire = closestFirePosition,
                config = config
            }.ScheduleParallel(state.Dependency);
            state.Dependency = fireExtuinguishJob;
            
            
            //Make the front guy find another fire 
            var TransitionManager = SystemAPI.GetSingletonEntity<Transition>();
            ECB.AddComponent<updateBotNearestTag>(TransitionManager);
        }
    }
}

[BurstCompile]
public partial struct FireExtuinguishJob: IJobEntity
{
    [ReadOnly] public float3 centerFire;
    [ReadOnly] public Config config;
    void Execute(ref Tile tile, in LocalTransform tileTransform)
    {
        if (math.abs(tileTransform.Position.x - centerFire.x) <= config.cellSize * config.splashRadius &&
            math.abs(tileTransform.Position.z - centerFire.z) <= config.cellSize * config.splashRadius)
        {
            tile.Temperature = 0.0f;
        }
    }
}