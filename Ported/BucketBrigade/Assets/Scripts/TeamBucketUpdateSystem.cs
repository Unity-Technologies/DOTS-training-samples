using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TeambotMovingSystem))]
[BurstCompile]
public partial struct TeamBucketSystem : ISystem
{
    // [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var teambotLookup = SystemAPI.GetComponentLookup<Teambot>();
        var localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        var config = SystemAPI.GetSingleton<Grid>();

        foreach (var (bucket, localTransform, pTMatrix, bucketEntity) in SystemAPI
                     .Query<RefRW<TeamBucket>, RefRW<LocalTransform>, RefRW<PostTransformMatrix>>().WithEntityAccess())
        {
            

             
            
             var teambotEntity = bucket.ValueRO.TargetTeambotEntity;
             var teambotPosition = localTransformLookup[teambotEntity].Position;
             var teambot = teambotLookup[teambotEntity];
             // Assign OMniBot to Bucket
             localTransform.ValueRW.Position = teambotPosition + new float3(0, config.BotScale / 2, 0);
             

             
            if (teambot.Role == TeamBotRole.WaterGatherer)
            {
                pTMatrix.ValueRW.Value = float4x4.Scale(new float3(
                    1, teambot.waterFillValue, 1));
            }
            else if (teambot.Role == TeamBotRole.FireDouser)
            {
                pTMatrix.ValueRW.Value = float4x4.Scale(new float3(1, 0, 1));

            }
        }
    }
}