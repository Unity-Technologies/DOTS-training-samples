using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct CarriageAspect : IAspect
{
    // Aspects can contain other aspects.
    readonly TransformAspect Transform;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    readonly RefRO<Carriage> CarriageComponent;
    
    readonly RefRO<CarriageBounds> ObjectBounds;

    // Properties like this are not mandatory, the Transform field could just have been made public instead.
    // But they improve readability by avoiding chains of "aspect.aspect.aspect.component.value.value".
    public float3 Position
    {
        get => Transform.LocalPosition;
        set => Transform.LocalPosition = value;
    }
    
    public float3 TrainPosition => CarriageComponent.ValueRO.TrainPosition;
    public float3 TrainDirection => CarriageComponent.ValueRO.TrainDirection;
    public quaternion TrainRotation => CarriageComponent.ValueRO.TrainRotation;

    public int Index => CarriageComponent.ValueRO.Index;
    public float Width => ObjectBounds.ValueRO.Width;

    public quaternion Rotation  
    {
        get => Transform.LocalRotation;
        set => Transform.LocalRotation = value;
    }
}