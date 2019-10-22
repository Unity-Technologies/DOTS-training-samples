using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

// Gravity system (no rotation)

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateAfter(typeof(MoverSystem))]
// ReSharper disable once InconsistentNaming
public class GravitySystem : JobComponentSystem
{
    private EntityQuery m_RockGroup;
    private EntityQuery m_TinCanGroup;

    // Position, Velocity, read only GravityStrength, FlyingTag
    protected override void OnCreate()
    {
        m_RockGroup = GetEntityQuery(typeof(Translation), typeof(Mover),
            ComponentType.ReadOnly<FlyingTag>(), ComponentType.ReadOnly<RockTag>());
        m_TinCanGroup = GetEntityQuery(typeof(Translation), typeof(Mover),
            ComponentType.ReadOnly<FlyingTag>(), ComponentType.ReadOnly<TinCanTag>());
    }

    [BurstCompile]
    struct GravityJob : IJobChunk
    {
        public float Time;
        public float GravityStrength;
        public ArchetypeChunkComponentType<Translation> Translation;
        public ArchetypeChunkComponentType<Mover> Mover;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var chunkTranslations = chunk.GetNativeArray(Translation);
            var chunkVelocities = chunk.GetNativeArray(Mover);
            for (var i = 0; i < chunkTranslations.Length; i++)
            {
                var pos = chunkTranslations[i].Value;
                var velocity = chunkVelocities[i].velocity;
                pos += velocity * Time;
                velocity.y -= GravityStrength * Time;

                chunkTranslations[i] = new Translation
                {
                    Value = pos
                };

                chunkVelocities[i] = new Mover
                {
                    velocity = velocity
                };
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var translation = GetArchetypeChunkComponentType<Translation>();
        var mover = GetArchetypeChunkComponentType<Mover>();

        var rockJob = new GravityJob
        {
            Time = Time.deltaTime,
            Translation = translation,
            Mover = mover,
            GravityStrength = RockManagerAuthoring.RockGravityStrength
        };

        var jobHandle = rockJob.Schedule(m_RockGroup, inputDeps);

        var tinCanJob = new GravityJob
        {
            Time = Time.deltaTime,
            Translation = translation,
            Mover = mover,
            GravityStrength = TinCanManagerAuthoring.TinCanGravityStrength
        };

        return tinCanJob.Schedule(m_TinCanGroup, jobHandle);
    }
}
