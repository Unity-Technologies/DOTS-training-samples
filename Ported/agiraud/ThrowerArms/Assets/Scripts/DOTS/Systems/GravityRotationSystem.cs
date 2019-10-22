using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Gravity rotation (for falling tin can)

// ReSharper disable once InconsistentNaming
public class GravityRotationSystem : JobComponentSystem
{
    private EntityQuery m_TinCanGroup;

    // Rotation, FlyingTag, TinCanTag, AngularVelocity
    protected override void OnCreate()
    {
        m_TinCanGroup = GetEntityQuery(typeof(Rotation), ComponentType.ReadOnly<FlyingTag>(),
            ComponentType.ReadOnly<TinCanTag>(), ComponentType.ReadOnly<AngularVelocity>());
    }

    [BurstCompile]
    struct RotationJob : IJobChunk
    {
        public float Time;
        public ArchetypeChunkComponentType<Rotation> Rotation;
        public ArchetypeChunkComponentType<AngularVelocity> AngularVelocity;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var chunkRotations = chunk.GetNativeArray(Rotation);
            var chunkAngularVelocities = chunk.GetNativeArray(AngularVelocity);
            for (var i = 0; i < chunkRotations.Length; i++)
            {
                var angularVelocity = chunkAngularVelocities[i].Value; // Test value : new float3(1f,0f,0f);
                var rotation = math.mul(quaternion.AxisAngle(angularVelocity, math.length(angularVelocity) * Time), chunkRotations[i].Value);
                chunkRotations[i] = new Rotation
                {
                    Value = rotation
                };
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var rotation = GetArchetypeChunkComponentType<Rotation>();
        var angularVelocity = GetArchetypeChunkComponentType<AngularVelocity>();

        var rotationJob = new RotationJob
        {
            Time = Time.deltaTime,
            Rotation = rotation,
            AngularVelocity = angularVelocity
        };

        return rotationJob.Schedule(m_TinCanGroup, inputDeps);
    }
}
