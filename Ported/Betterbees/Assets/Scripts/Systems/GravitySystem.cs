using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(BeeSystem))]
[UpdateBefore(typeof(FoodSystem))]
[UpdateBefore(typeof(DecaySystem))]
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

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        GravityJob gravityJob = new GravityJob
        {
            lowerBounds = -config.bounds.y,
            acceleration = config.gravity * SystemAPI.Time.DeltaTime,
            ecb = ecb.AsParallelWriter()
        };
        gravityJob.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(GravityComponent))]
    private partial struct GravityJob : IJobEntity
    {
        public float lowerBounds;
        public float3 acceleration;
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute(ref LocalTransform transform, ref VelocityComponent velocity, Entity entity, [ChunkIndexInQuery] int chunkIndex)
        {
            if (transform.Position.y > lowerBounds)
            {
                velocity.Velocity += acceleration;
            }
            else
            {
                velocity.Velocity = 0;
                transform.Position.y = lowerBounds;

                ecb.SetComponentEnabled<GravityComponent>(chunkIndex, entity, false);
            }
        }
    }
}