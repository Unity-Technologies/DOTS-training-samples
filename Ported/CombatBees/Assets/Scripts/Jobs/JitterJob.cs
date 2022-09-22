using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[WithAll(typeof(TargetId))]
partial struct JitterJob : IJobEntity {

    public int randomBase;
    public float deltaTime;
    public float jitter;
    public float damping;

    void Execute([EntityInQueryIndex]int index, ref Velocity velocity) {
        velocity.Value += Random.CreateFromIndex((uint) (randomBase * index)).NextFloat3Direction() * (jitter * deltaTime);
        velocity.Value *= (1f - (damping * deltaTime)) ;
    }
}
