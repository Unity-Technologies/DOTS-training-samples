using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct JitterJob : IJobEntity {

    public Random random;
    public float deltaTime;
    public float jitter;
    public float damping;

    void Execute(ref Velocity velocity, in IsHolding isHolding) {
        velocity.Value += random.NextFloat3Direction() * (jitter * deltaTime);
        velocity.Value *= (1f - (damping * deltaTime)) ;
    }
}
