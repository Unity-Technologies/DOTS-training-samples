using Unity.Burst;
using Unity.Entities;

[BurstCompile]
partial struct DecayVelocityJob : IJobEntity {

    void Execute(ref Decay decay, ref Velocity velocity) {
        velocity.Value *= decay.velocityChange;
        decay.velocityChange = 1.0f;
    }
}

[BurstCompile]
partial struct DecayTimerJob : IJobEntity {

    public float deltaTime;

    public EntityCommandBuffer.ParallelWriter ecb;
    
    void Execute(Entity entity,[EntityInQueryIndex] int index, ref DecayTimer decay) {
        decay.Value -= deltaTime;
        if (decay.Value < 0) {
            ecb.DestroyEntity(index, entity);
        }
    }
}