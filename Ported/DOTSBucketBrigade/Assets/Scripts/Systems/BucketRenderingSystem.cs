using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class BucketRenderingSystem : SystemBase
{

    private static readonly float4 BUCKET_EMPTY_COLOUR = new float4(1.0f, 0.41037738f, 0.45895237f, 1.0f);
    private static readonly float4 BUCKET_FULL_COLOUR = new float4(0.0f, 0.9797907f, 1.0f, 1.0f);
    private static readonly float4 WATER_COLOUR = new float4(0.0f, 0.62122846f, 1.0f, 1.0f);


    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Bucket>();
    }

    protected override void OnUpdate()
    {

        Entities
            .WithAll<Bucket>()
            .ForEach((Entity entity, ref URPMaterialPropertyBaseColor color, ref NonUniformScale scale, ref Volume volume) =>
            {

                if (volume.Value == 0.0f)
                {
                    color = new URPMaterialPropertyBaseColor { Value = BUCKET_EMPTY_COLOUR };
                    float emptyBucketScale = 0.2f;
                    scale.Value = new float3(emptyBucketScale, emptyBucketScale, emptyBucketScale);
                }
                else if (volume.Value > 0.0f & volume.Value <= 0.01f)
                {
                    color = new URPMaterialPropertyBaseColor { Value = WATER_COLOUR };
                    float fillingBucketScale = 0.4f;
                    scale.Value = new float3(fillingBucketScale, fillingBucketScale, fillingBucketScale);
                }
                else if (volume.Value > 0.01f & volume.Value <= 1)
                {
                    color = new URPMaterialPropertyBaseColor { Value = math.lerp(WATER_COLOUR, BUCKET_FULL_COLOUR, volume.Value) };
                    float fillingBucketScale = 0.4f;
                    float filledBucketScale = 1.4f;
                    scale.Value = new float3(math.lerp(fillingBucketScale, filledBucketScale, volume.Value), math.lerp(fillingBucketScale, filledBucketScale, volume.Value), math.lerp(fillingBucketScale, filledBucketScale, volume.Value));
                }

            }).Run();

    }
}
