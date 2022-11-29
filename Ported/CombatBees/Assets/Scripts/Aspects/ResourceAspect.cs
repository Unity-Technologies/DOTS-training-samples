using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ResourceAspect : IAspect
{
    public readonly Entity Self;

    readonly TransformAspect Transform;
    
    readonly RefRW<ResourceComponent> resourceComponent;
    readonly RefRW<ResourceCarriedComponent> resourceCarriedState;
    readonly RefRW<ResourceDroppedComponent> resourceDroppedState;
    
    public float3 Position
    {
        get => Transform.WorldPosition;
        set => Transform.WorldPosition = value;
    }
}