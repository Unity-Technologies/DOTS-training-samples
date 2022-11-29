using Unity.Entities;
using Unity.Transforms;

readonly partial struct TrainSpeedControllerAspect : IAspect
{
    // Aspects can contain other aspects.
    readonly RefRO<WorldTransform> Transform;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    readonly RefRO<SpeedComponent> SpeedComponent;
}