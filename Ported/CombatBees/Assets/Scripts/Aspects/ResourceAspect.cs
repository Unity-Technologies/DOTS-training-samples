using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ResourceAspect : IAspect
{
    public readonly Entity Self;

    readonly TransformAspect Transform;
    
    readonly RefRW<Resource> resourceComponent;
    readonly RefRW<ResourceCarried> resourceCarriedState;
    readonly RefRW<ResourceDropped> resourceDroppedState;
    
    public float3 Position
    {
        get => Transform.WorldPosition;
        set => Transform.WorldPosition = value;
    }
}