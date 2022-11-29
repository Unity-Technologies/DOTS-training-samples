using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct TrainSpawnAspect : IAspect
{
    // An Entity field in an aspect provides access to the entity itself.
    // This is required for registering commands in an EntityCommandBuffer for example.
    public readonly Entity Self;

    // Aspects can contain other aspects.
    readonly TransformAspect Transform;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    readonly RefRO<TrainSpawn> Spawn;

    public float3 Position
    {
        get => Transform.LocalPosition;
        set => Transform.LocalPosition = value;
    }

    public Entity Prefab => Spawn.ValueRO.CarriageSpawn;

    public int Amount => Spawn.ValueRO.CarriageCount;
}