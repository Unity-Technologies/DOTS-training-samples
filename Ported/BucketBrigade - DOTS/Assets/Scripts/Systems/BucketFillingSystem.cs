using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BucketSpawningSystem))]
[UpdateAfter(typeof(WaterSpawningSystem))]
[BurstCompile]
public partial struct BucketFillingSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    { 
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<Water>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float minDist;
        var config = SystemAPI.GetSingleton<Config>();

        //For each bucket with FillingTag enabled
        foreach (var (bucketParams, bucketTransform, bucketColor, bucket) in SystemAPI.Query<RefRW<Bucket>, RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>().WithEntityAccess().WithAll<FillingTag>())
        {
            minDist = float.MaxValue;
            Entity closestWater = default;
            
            //Get closest water
            foreach (var (water,waterTransform, waterEntity) in SystemAPI.Query<RefRW<Water>,LocalTransform>().WithEntityAccess())
            {
                var dist = Vector3.Distance(waterTransform.Position, bucketTransform.ValueRO.Position);
                if (water.ValueRO.CurrCapacity >= config.bucketFillRate) //Check if it has water in it
                {
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestWater = waterEntity;
                    }
                }
            }

            if (closestWater == default) return;

            //Add water to the bucket
            bucketParams.ValueRW.CurrCapacity += config.bucketFillRate;
            
            //Decrease water in the watersource
            var test = SystemAPI.GetComponent<Water>(closestWater);
            test.CurrCapacity -= config.bucketFillRate;
            SystemAPI.SetComponent(closestWater, test);

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
            
            //Check if the bucket is full and if so change tags
            if (bucketParams.ValueRO.CurrCapacity >= bucketParams.ValueRO.MaxCapacity)
            {
                bucketParams.ValueRW.CurrCapacity = bucketParams.ValueRW.MaxCapacity;
                SystemAPI.SetComponentEnabled<FillingTag>(bucket, false);
                SystemAPI.SetComponentEnabled<FullTag>(bucket, true);
            }
        }
    }
}