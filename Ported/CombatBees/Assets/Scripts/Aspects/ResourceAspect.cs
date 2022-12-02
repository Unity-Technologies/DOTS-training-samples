using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ResourceAspect : IAspect
{
    public readonly Entity Self;
    public readonly TransformAspect Transform;
    
    readonly RefRW<Resource> resourceComponent;
    
    public float3 Position
    {
        get => Transform.WorldPosition;
        set => Transform.WorldPosition = value;
    }
    
    public float3 velocity
    {
        get => resourceComponent.ValueRO.velocity;
        set => resourceComponent.ValueRW.velocity = value;
    }
    
    public bool droppedEnabled
    {
        get => resourceComponent.ValueRO.droppedEnabled;
        set => resourceComponent.ValueRW.droppedEnabled = value;
    }
    
    public bool carriedEnabled
    {
        get => resourceComponent.ValueRO.carriedEnabled;
        set => resourceComponent.ValueRW.carriedEnabled = value;
    }
    
    public float3 ownerVelocity
    {
        get => resourceComponent.ValueRO.ownerVelocity;
        set => resourceComponent.ValueRW.ownerVelocity = value;
    }
    
    public float3 ownerPosition
    {
        get => resourceComponent.ValueRO.ownerPosition;
        set => resourceComponent.ValueRW.ownerPosition = value;
    }
    
    public Entity carriedBee => resourceComponent.ValueRO.ownerBee;
}