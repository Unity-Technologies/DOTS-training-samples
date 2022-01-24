using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
struct GameConstants : IComponentData
{
    public int2 FieldSize; // TODO: Limit to 2^n equal sided sizes?
    public float BucketSpawnDensity;
    public float FireSpawnDensity;

    public float FireFighterMovementSpeedNoBucket;
    public float FireFighterMovementSpeedBucket;
    public float FireFighterBucketPickupRadius;

    public float BucketFillRate;

    // TODO: How buckets affect fire
    // TODO: How fire propogates and grows
    public float FireHeatFlashPoint; // When ground catches fire (0.2 in original code)
    public float FireHeatTransferRadius;
    public float FireHeatTransferRate;

    public Entity FlamePrefab;
}

