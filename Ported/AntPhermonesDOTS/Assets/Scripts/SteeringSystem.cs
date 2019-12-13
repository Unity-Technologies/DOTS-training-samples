using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

struct Velocity: IComponentData
{
    public float Rotation;
    public float Speed;
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(MoveSystem))]
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

        Obstacle obstacle = GetSingleton<Obstacle>();

        var jobHandle = Entities
            .WithName("SteeringSystem")
            .WithReadOnly(obstacle)
            .WithAll<AntTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Velocity velocity, in Translation translation) =>
            {
                var rotationBefore = velocity.Rotation;
                
                var randomStrength = 0.15f;
                uint seed = (uint)(entityInQueryIndex * 0x9F6ABC1) + frameCount;
                uint scrambledSeed = WangHash(seed);
                if (scrambledSeed == 0) scrambledSeed = 1;
                var rng = new Random(scrambledSeed);
                float rand = rng.NextFloat(-randomStrength, randomStrength);
                velocity.Rotation += rand;
                velocity.Speed = 0.2f;
                
                ///////// Obstacle Steering
                var distance = 1.5f;
                int output = 0;

                for (int i = -1; i <= 1; i+=2) {
                    float angle = rotationBefore + i * Mathf.PI * .25f; // 45 deg
                    var test = translation.Value.xy + new float2(math.cos(angle), math.sin(angle)) * distance;

                    var mapSize = 128;
                    var tileCount = 32;
                    var tileSize = mapSize / tileCount;
            
                    var tilePos = ObstacleHelper.WorldPosToTilePos(test, mapSize, tileSize);
                    var tileIndex = ObstacleHelper.TileIndex(tilePos, tileCount);
                    var bitFieldIndex = ObstacleHelper.BitFieldIndex(tileIndex);
                    var bitIndex = ObstacleHelper.BitIndex(tileIndex);

                    if(obstacle.Blob.Value.TileOccupancy[bitFieldIndex].IsSet(bitIndex) && output == 0)
                    {
                        output -= i;
                    }
                }
                velocity.Rotation += output * 0.2f;
            }).Schedule(inputDeps);
        return jobHandle;
    }
}
