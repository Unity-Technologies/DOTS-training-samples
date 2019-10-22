using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Windows.WebCam;

// Gravity system (no rotation)

// ReSharper disable once InconsistentNaming
public class GravitySystem : JobComponentSystem
{
    private EntityQuery m_Group;

    // Position, Velocity, read only GravityStrength, FlyingTag
    protected override void OnCreate()
    {
        m_Group = GetEntityQuery(typeof(Translation), typeof(Mover), ComponentType.ReadOnly<GravityStrength>(),
            ComponentType.ReadOnly<FlyingTag>());
    }

    [BurstCompile]
    struct GravityJob : IJobChunk
    {
        public float Time;
        public ArchetypeChunkComponentType<Translation> Translation;
        public ArchetypeChunkComponentType<Mover> Mover;
        [ReadOnly] public ArchetypeChunkSharedComponentType<GravityStrength> GravityStrength;
        [ReadOnly] public ArchetypeChunkComponentType<FlyingTag> FlyingTag;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var chunkTranslations = chunk.GetNativeArray(Translation);
            for (var i = 0; i < chunkTranslations.Length; i++)
            {
                var pos = chunkTranslations[i].Value - 1;

                chunkTranslations[i] = new Translation
                {
                    Value = pos
                };
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var translation = GetArchetypeChunkComponentType<Translation>();
        var mover = GetArchetypeChunkComponentType<Mover>();
        var gravityStrength = GetArchetypeChunkSharedComponentType<GravityStrength>();
        var flyingTag = GetArchetypeChunkComponentType<FlyingTag>(true);

        var job = new GravityJob
        {
            Time = Time.deltaTime,
            Translation = translation,
            Mover = mover,
            GravityStrength = gravityStrength,
            FlyingTag = flyingTag
        };

        return job.Schedule(m_Group, inputDeps);
    }
}