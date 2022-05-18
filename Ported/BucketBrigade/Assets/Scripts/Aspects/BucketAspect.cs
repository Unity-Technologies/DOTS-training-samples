using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

readonly partial struct BucketAspect : IAspect<BucketAspect>
{
    //Buckets have a few properties. Transform and WaterLevel. The latter goes from 0 to 1, a float. 
    private readonly RefRW<Bucket> Bucket;
    private readonly TransformAspect Transform;
    public readonly RefRW<Scale> _scale;

    private readonly RefRW<URPMaterialPropertyBaseColor> m_BaseColor;

    public float3 Position
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }

    public float FillLevel {
        get => Bucket.ValueRO.fillLevel;
        set => Bucket.ValueRW.fillLevel = value;
    }

    public float Scale {
        get => _scale.ValueRO.Value;
        set => _scale.ValueRW.Value = value;
    }

    public float4 Color {
        get => m_BaseColor.ValueRO.Value;
        set => m_BaseColor.ValueRW.Value = value;
    }
}