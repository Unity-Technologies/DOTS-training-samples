using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct PlatformQueueAspect : IAspect
{
    readonly TransformAspect Transform;
    public readonly RefRO<PlatformQueue> PlatformQueue;
    readonly RefRO<PlatformId> platformId;

    public float3 Position => Transform.WorldPosition;
    public float3 Direction => Transform.Forward;
    public int PlatformId => platformId.ValueRO.Value;
}