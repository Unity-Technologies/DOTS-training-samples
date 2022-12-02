using Unity.Entities;
using Unity.Transforms;

public readonly partial struct CarriageMovePassengerAspect : IAspect
{
    // Aspects can contain other aspects.
    public readonly RefRO<LocalTransform> Transform;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    public readonly RefRO<CarriageSeatsPositions> CarriageSeats;

    public readonly DynamicBuffer<CarriagePassengers> Passengers;
}