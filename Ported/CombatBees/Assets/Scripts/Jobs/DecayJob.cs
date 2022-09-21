using Unity.Burst;
using Unity.Entities;

[BurstCompile]
partial struct DecayJob : IJobEntity {

    void Execute(ref Decay decay, ref Velocity velocity) {
        velocity.Value *= decay.velocityChange;
        decay.velocityChange = 1.0f;
    }
}