using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotRenderingSystem : SystemBase
{

    private static readonly float4 BOT_PASS_EMPTY_COLOUR = new float4(0.9339623f, 0.7533375f, 0.9255219f, 1.0f);
    private static readonly float4 BOT_SCOOP_COLOUR = new float4(0.0f, 1.0f, 0.1510973f, 1.0f);

    private static readonly float4 BOT_PASS_FULL_COLOUR = new float4(0.7729952f, 0.9245283f, 0.73700607f, 1.0f);
    private static readonly float4 BOT_THROW_COLOUR = new float4(1.0f, 0.4764151f, 0.9354846f, 1.0f);
    private static readonly float4 BOT_FETCHER_COLOR = new float4(0.0f, 1.0f, 0.0f, 1.0f);

    //private static readonly float4 BOT_OMNIBOT = new float4(0.0f, 0.0f, 0.0f, 1.0f); Unused???

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<FullBucketer>();
        RequireSingletonForUpdate<EmptyBucketer>();
        RequireSingletonForUpdate<BucketFetcher>();
    }

    protected override void OnUpdate()
    {

        Entities
            .WithAll<FullBucketer>()
            .ForEach((Entity entity,ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color) =>
            {
                color = new URPMaterialPropertyBaseColor { Value = BOT_PASS_EMPTY_COLOUR };
                //scale.Value = new float3(0.4f, 5.4f, 0.4f);

            }).Run();


        Entities
            .WithAll<EmptyBucketer>()
            .ForEach((Entity entity, ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color) =>
            {
                color = new URPMaterialPropertyBaseColor { Value = BOT_PASS_FULL_COLOUR };
                //scale.Value = new float3(0.4f, 12.4f, 0.4f);

            }).Run();

        Entities
            .WithAll<BucketFetcher>()
            .ForEach((Entity entity, ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color) =>
            {
                color = new URPMaterialPropertyBaseColor { Value = BOT_FETCHER_COLOR };
                //scale.Value = new float3(1.4f, 20.4f, 1.4f);
            }).Run();

        Entities
            .WithAll<EmptyBucketer>()
            .WithAny<LastInLine>()
            .ForEach((Entity entity, ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color) =>
            {
                color = new URPMaterialPropertyBaseColor { Value = BOT_SCOOP_COLOUR };
                //scale.Value = new float3(1.4f, 20.4f, 1.4f);
            }).Run();

        Entities
            .WithAll<FullBucketer>()
            .WithAny<LastInLine>()
            .ForEach((Entity entity, ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color) =>
            {
                color = new URPMaterialPropertyBaseColor { Value = BOT_THROW_COLOUR };
                //scale.Value = new float3(1.4f, 20.4f, 1.4f);
            }).Run();

    }
}
