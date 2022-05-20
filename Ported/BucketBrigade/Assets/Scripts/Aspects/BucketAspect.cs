using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

readonly partial struct BucketAspect : IAspect<BucketAspect>
{
    public readonly Entity Self;
    
    //Buckets have a few properties. Transform and WaterLevel. The latter goes from 0 to 1, a float. 
    private readonly RefRW<Bucket> Bucket;
    private readonly TransformAspect Transform;
    private readonly RefRW<NonUniformScale> _scale;

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
        get => _scale.ValueRO.Value.x;
        set => _scale.ValueRW.Value = value;
    }

    public float4 Color {
        get => m_BaseColor.ValueRO.Value;
        set => m_BaseColor.ValueRW.Value = value;
    }

    public Fetcher Holder {
        get => Bucket.ValueRO.holder;
        set => Bucket.ValueRW.holder = value;
    }

    public BucketInteractions Interactions {
        get => Bucket.ValueRO.Interactions;
        set => Bucket.ValueRW.Interactions = value;
    }
    
    public int NbOfFiremen {
        get => Bucket.ValueRO.NbOfFiremen;
        set => Bucket.ValueRW.NbOfFiremen = value;
    }
    
    public int CurrentFiremanIdx {
        get => Bucket.ValueRO.CurrentFiremanIdx;
        set => Bucket.ValueRW.CurrentFiremanIdx = value;
    }
    
    public int AwaiterCount {
        get => Bucket.ValueRO.AwaiterCount;
        set => Bucket.ValueRW.AwaiterCount = value;
    }
    
}