using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MovementJob job = new MovementJob()
        {
            dt = SystemAPI.Time.DeltaTime
        };
        job.ScheduleParallel();
    }


    [BurstCompile]
    private partial struct MovementJob : IJobEntity
    {
        public float dt;

        public void Execute(ref LocalTransform transform, ref VelocityComponent velocity)
        {
            transform.Position += velocity.Velocity * dt;
        }
    }
}
