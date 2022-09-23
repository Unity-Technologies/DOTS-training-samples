using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[WithAll(typeof(Decay))]
[WithNone(typeof(DecayTimer))]
partial struct ResourceDespawnJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute(Entity e, [EntityInQueryIndex] int idx)
    {
        ECB.DestroyEntity(idx, e);
    }
}

[BurstCompile]
[WithAll(typeof(Decay))]
partial struct BeeDespawnJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    public float DeltaTime;
    
    void Execute(Entity e, [EntityInQueryIndex] int idx, ref DecayTimer ttl)
    {
        ttl.Value -= DeltaTime;
        if (ttl.Value < 0f)
        {
            ECB.DestroyEntity(idx, e);
        }
    }
}