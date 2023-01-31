using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

struct SaschaMovementQueueInstruction
{
    public float3 Destination;
    public Entity Platform;
}

struct SaschaMovementQueue : IComponentData
{
    public NativeQueue<SaschaMovementQueueInstruction> QueuedInstructions;
}

