using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, speed, direction) in SystemAPI.Query<TransformAspect, Speed, Direction>())
        {
            var dir3D = new float3(direction.Value.x, 0f, direction.Value.y);
            var velocity = dir3D * speed.Value;
            transform.WorldPosition += velocity * SystemAPI.Time.DeltaTime;
        }
    }
}