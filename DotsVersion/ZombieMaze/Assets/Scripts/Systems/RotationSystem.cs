using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct RotationSystem : ISystem
{
    // Every function defined by ISystem has to be implemented even if empty.
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    // Every function defined by ISystem has to be implemented even if empty.
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // See note above regarding the [BurstCompile] attribute.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // The amount of rotation around Y required to do 360 degrees in 2 seconds.
        var rotation = quaternion.RotateY(state.Time.DeltaTime * math.PI);

        // The classic C# foreach is what we often refer to as "Idiomatic foreach" (IFE).
        // Aspects provide a higher level interface than directly accessing component data.
        // Using IFE with aspects is a powerful and expressive way of writing main thread code.
        foreach (var transform in SystemAPI.Query<TransformAspect>())
        {
            transform.RotateWorld(rotation);
        }
    }
}
