using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
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

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        GravityJob gravityJob = new GravityJob
        {
            config = config,
            ecb = ecb.AsParallelWriter(),
            deltaTime = SystemAPI.Time.DeltaTime
        };
        gravityJob.ScheduleParallel();
    }

    [BurstCompile]
    private partial struct GravityJob : IJobEntity
    {
        public Config config;
        public EntityCommandBuffer.ParallelWriter ecb;
        public float deltaTime;

        public void Execute(in GravityComponent gravityComponent, ref LocalTransform transform, ref VelocityComponent velocity, Entity entity, [ChunkIndexInQuery] int chunkIndex)
        {
            if (transform.Position.y > -config.bounds.y)
            {
                velocity.Velocity += config.gravity * deltaTime;
            }
            else
            {
                velocity.Velocity = 0;
                transform.Position.y = -config.bounds.y;

                ecb.RemoveComponent<GravityComponent>(chunkIndex, entity);
            }
        }
    }
}