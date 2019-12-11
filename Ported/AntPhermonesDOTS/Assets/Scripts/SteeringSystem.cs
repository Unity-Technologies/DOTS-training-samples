using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

struct Velocity: IComponentData
{
    public float Rotation;
    public float Speed;
}

public class SteeringSystem: JobComponentSystem
{
    public uint FrameCount = 0;

    static uint WangHash(uint seed)
    {
        seed = (seed ^ 61) ^ (seed >> 16);
        seed *= 9;
        seed ^= (seed >> 4);
        seed *= 0x27d4eb2d;
        seed ^= (seed >> 15);
        return seed;
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        FrameCount += 1;
        var frameCount = FrameCount;
        
        var jobHandle = Entities
            .WithName("SpawnerSystem")
            .WithAll<AntTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Velocity velocity) =>
            {
                uint seed = (uint)(entityInQueryIndex * 0x9F6ABC1) + frameCount + 1;
                uint scrambledSeed = WangHash(seed);
                if (scrambledSeed == 0) scrambledSeed = 1;
                var rng = new Random(scrambledSeed);
                float rand = rng.NextFloat(-0.3f, 0.3f);
                velocity.Rotation += rand;
                velocity.Speed = 0.03f;
            }).Schedule(inputDeps);
        return jobHandle;
    }
}
