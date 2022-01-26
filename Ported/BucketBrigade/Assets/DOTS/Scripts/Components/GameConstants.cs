using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
struct GameConstants : IComponentData
{
    public int2 FieldSize; // TODO: Limit to 2^n equal sided sizes?
    public float FireSpawnDensity;

    public Entity FireFighterPrefab;

    public float FireFighterMovementSpeedNoBucket;
    public float FireFighterMovementSpeedBucket;
    public float FireFighterBucketPickupRadius;

    public float BucketSpawnDensity;
    public float BucketFillRate;
    public Entity BucketPrefab;
    public float4 BucketEmpty;
    public float4 BucketFilled;
    public float BucketExtinguishRadius;
    public float BucketCoolingRate;
    public float BucketEmptyScale;
    public float BucketFilledScale;

    // TODO: How buckets affect fire
    // TODO: How fire propogates and grows
    public float FireHeatFlashPoint; // When ground catches fire (0.2 in original code)
    public int FireHeatTransferRadius;
    public float FireHeatTransferRate;
    public float FireSimUpdateRate;
    public float4 FireMaxColor;
    public float4 FireMinColor;
    public float FireOSCRange;
    public Entity FlamePrefab;

    public Entity TeamPrefab;
    public int TeamCountAtStart;

    public float LakeMaxVolume;
}

