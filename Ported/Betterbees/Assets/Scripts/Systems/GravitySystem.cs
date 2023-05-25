using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial struct GravitySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        using (var commandBuffer = new EntityCommandBuffer(Allocator.TempJob))
        {
            foreach (var (_, transform, velocity, entity) in SystemAPI
                .Query<RefRO<GravityComponent>, RefRW<LocalTransform>, RefRW<VelocityComponent>>()
                .WithEntityAccess())
            {
                if (transform.ValueRO.Position.y > -config.bounds.y)
                {
                    velocity.ValueRW.Velocity += config.gravity * SystemAPI.Time.DeltaTime;
                }
                else
                {
                    velocity.ValueRW.Velocity = 0;
                    transform.ValueRW.Position.y = -config.bounds.y;

                    commandBuffer.RemoveComponent<GravityComponent>(entity);
                }
            }

            commandBuffer.Playback(state.EntityManager);
        }
    }
}