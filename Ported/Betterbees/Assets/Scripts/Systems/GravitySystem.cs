using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(MovementSystem))]
public partial struct GravitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        using (var commandBuffer = new EntityCommandBuffer(Allocator.TempJob))
        {
            foreach (var (_, transform, velocity, entity) in SystemAPI
            .Query<RefRO<GravityComponent>, RefRW<LocalTransform>, RefRW<VelocityComponent>>()
            .WithEntityAccess())
            {
                if (transform.ValueRO.Position.y > -Config.bounds.y)
                {
                    velocity.ValueRW.Velocity += Config.gravity * SystemAPI.Time.DeltaTime;
                }
                else
                {
                    velocity.ValueRW.Velocity = 0;
                    transform.ValueRW.Position.y = -Config.bounds.y;

                    commandBuffer.RemoveComponent<GravityComponent>(entity);
                }
            }

            commandBuffer.Playback(state.EntityManager);
        }
    }
}