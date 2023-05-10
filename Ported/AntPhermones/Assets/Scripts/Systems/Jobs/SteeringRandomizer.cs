using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct SteeringRandomizerJob : IJobEntity
{
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Random> rngs;
    public float randomSteering;

    [NativeSetThreadIndex] private int threadId;

    [BurstCompile]
    public void Execute(ref Direction direction)
    {
        var rng = rngs[threadId];
        direction.direction += rng.NextFloat(-randomSteering, randomSteering);
        rngs[threadId] = rng; // this is necessary because its a struct so we must update the state
    }
}
