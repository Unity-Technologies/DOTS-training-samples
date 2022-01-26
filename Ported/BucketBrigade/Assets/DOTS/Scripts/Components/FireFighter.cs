using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct FireFighter : IComponentData { }

public struct HoldingBucket : IComponentData
{
    public Entity HeldBucket;
}

public struct BucketFetcher : IComponentData {
    public float3 LakePosition;
    public Entity Lake;
}


public struct HoldsEmptyBucket : IComponentData { }
public struct HoldsFullBucket : IComponentData { }
public struct HoldsBucketBeingFilled : IComponentData { public Entity Lake; }