using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BucketSpawningSystem))]
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

        //For each bucket with EmptyingTag enabled
        foreach (var (bucketParams, bucketTransform, bucketColor, bucket) in SystemAPI.Query<RefRW<Bucket>, RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>().WithEntityAccess().WithAll<EmptyingTag>())
        {
            minDist = float.MaxValue;
            Entity closestFire = Entity.Null;
            
            //Get closest fire
            foreach (var (fire, fireTransform, fireEntity) in SystemAPI.Query<RefRW<OnFire>,LocalTransform>().WithEntityAccess())
            {
                //get the distance
                var dist = Vector3.Distance(fireTransform.Position, bucketTransform.ValueRO.Position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestFire = fireEntity;
                }
            }

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
            var closestFirePosition = SystemAPI.GetComponent<LocalTransform>(closestFire);
            var tileDiagonal = config.cellSize * math.sqrt(2);
            foreach (var (tile, tileTransform, tileEntity) in SystemAPI.Query<RefRW<Tile>,LocalTransform>().WithEntityAccess())
            {
                //get the distance
                var dist = Vector3.Distance(tileTransform.Position, closestFirePosition.Position);
               
                if (dist <= tileDiagonal)
                {
                    var tempTile = SystemAPI.GetComponent<Tile>(tileEntity);
                    tempTile.Temperature = 0;
                    SystemAPI.SetComponent(tileEntity, tempTile);
                }
            }
        }
    }
}