using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Burst;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TransformSystemGroup))]
public class ParticleUpdateSystem : JobComponentSystem
{
    private EntityQuery m_Particles;
    private EntityQuery m_DistanceFields;
    private static Unity.Mathematics.Random m_RandomNumberGenerator;
    private NativeArray<float> m_ParticleDistancesFromIsosurface;

    protected override void OnCreateManager()
    {
        m_Particles = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadWrite<Velocity>(), ComponentType.ReadWrite<Color>(), ComponentType.ReadWrite<LocalToWorld>(), ComponentType.ReadOnly<EmitterProperties>() },
        });

        m_DistanceFields = GetEntityQuery(ComponentType.ReadOnly(typeof(DistanceField)));
        m_RandomNumberGenerator = new Unity.Mathematics.Random(747);
    }

    protected override void OnDestroyManager()
    {
        if (m_ParticleDistancesFromIsosurface.IsCreated)
        {
            m_ParticleDistancesFromIsosurface.Dispose();
        }
    }

    [BurstCompile]
    [RequireComponentTag(typeof(EmitterProperties))]
    struct SimulateParticlesJob : IJobForEachWithEntity<LocalToWorld, Velocity>
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public EmitterProperties jobSettings;
        [ReadOnly] public Unity.Mathematics.Random jobRNG;
        [ReadOnly] public DistanceFieldModel jobDistanceFieldModel;
        public NativeArray<float> jobDistancesFromIsoSurface;

        public void Execute(Entity entity, int index, ref LocalToWorld localToWorld, ref Velocity velocity)
        {
            float3 normal;
            jobDistancesFromIsoSurface[index] = DistanceFieldMathUtil.GetDistance(localToWorld.Position.x, localToWorld.Position.y, localToWorld.Position.z, deltaTime, jobDistanceFieldModel, out normal);

            velocity.Value -= math.normalize(normal) * jobSettings.attraction * math.clamp(jobDistancesFromIsoSurface[index], -1f, 1f);
            velocity.Value += jobRNG.NextFloat3Direction() * jobSettings.jitter;
            velocity.Value *= .99f;

            localToWorld = new LocalToWorld
            {
                Value = float4x4.TRS(
                        new float3(localToWorld.Position + velocity.Value),
                        quaternion.LookRotationSafe(velocity.Value, math.up()),
                        new float3(.1f, .01f, math.max(.1f, math.length(velocity.Value) * jobSettings.speedStretch)))
            };
        }
    }

    [BurstCompile]
    [RequireComponentTag(typeof(EmitterProperties))]
    struct SimulateParticleColorsJob : IJobForEachWithEntity<Color>
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public EmitterProperties jobSettings;
        [ReadOnly] public PropertyColorsAsFloat4s jobPropertyColorFloat4s;
        [ReadOnly] public NativeArray<float> jobDistancesFromIsoSurface;

        public void Execute(Entity entity, int index, ref Color color)
        {
            float4 targetColor;
            if (jobDistancesFromIsoSurface[index] > 0f)
            {
                targetColor = math.lerp(jobPropertyColorFloat4s.surfaceColor, jobPropertyColorFloat4s.exteriorColor, jobDistancesFromIsoSurface[index] / jobSettings.exteriorColorDist);
            }
            else
            {
                targetColor = math.lerp(jobPropertyColorFloat4s.surfaceColor, jobPropertyColorFloat4s.interiorColor, -jobDistancesFromIsoSurface[index] / jobSettings.interiorColorDist);
            }

            color.Value = math.lerp(color.Value, targetColor, deltaTime * jobSettings.colorStiffness);
        }
    }

    // List of different instances of this particular type of shared component data.
    // e.g. if I had another type of particle (i.e. a different prefab) with different values in its EmitterProperties component,
    // that component would be a separate element in this list.
    // In this example, there's only 1 valid EmitterProperties, but the code shows how to handle more than one.
    private List<EmitterProperties>      m_UniqueParticlePropertiesEntries = new List<EmitterProperties>(10);

    internal struct PropertyColorsAsFloat4s
    {
        public float4 surfaceColor;
        public float4 interiorColor;
        public float4 exteriorColor;

        public PropertyColorsAsFloat4s(UnityEngine.Color surface, UnityEngine.Color interior, UnityEngine.Color exterior)
        {
            surfaceColor = new float4(surface.r, surface.g, surface.b, surface.a);
            interiorColor = new float4(interior.r, interior.g, interior.b, interior.a);
            exteriorColor = new float4(exterior.r, exterior.g, exterior.b, exterior.a);
        }
    }
    private PropertyColorsAsFloat4s m_PropertyColorsAsFloat4s;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var df = GetSingleton<DistanceField>();

        EntityManager.GetAllUniqueSharedComponentData(m_UniqueParticlePropertiesEntries);

        // Ignore typeIndex 0, can't use the default for anything meaningful.
        for (int entryIndex = 1; entryIndex < m_UniqueParticlePropertiesEntries.Count; entryIndex++)
        {
            var settings = m_UniqueParticlePropertiesEntries[entryIndex];
            m_Particles.SetFilter(settings);
            if (!m_ParticleDistancesFromIsosurface.IsCreated)
            {
                m_ParticleDistancesFromIsosurface = new NativeArray<float>(m_Particles.CalculateLength(), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_PropertyColorsAsFloat4s = new PropertyColorsAsFloat4s(settings.surfaceColor, settings.interiorColor, settings.exteriorColor);
            }

            var simulateParticlesJob = new SimulateParticlesJob()
            {
                deltaTime = Time.deltaTime,
                jobSettings = settings,
                jobRNG = m_RandomNumberGenerator,
                jobDistanceFieldModel = df.model,
                jobDistancesFromIsoSurface = m_ParticleDistancesFromIsosurface
            };
            var simulateParticlesJobHandle = simulateParticlesJob.Schedule(m_Particles, inputDeps);

            var simulateColorsJob = new SimulateParticleColorsJob()
            {
                deltaTime = Time.deltaTime,
                jobSettings = settings,
                jobPropertyColorFloat4s = m_PropertyColorsAsFloat4s,
                jobDistancesFromIsoSurface = m_ParticleDistancesFromIsosurface
            };
            // Simulating the colors depends on the jobDistancesFromIsoSurface output from simulateParticlesJob.
            var simulateColorsJobHandle = simulateColorsJob.Schedule(m_Particles, simulateParticlesJobHandle);

            // Combine all the job handles from this system so other systems can wait on this one if needed.
            var jobHandles = new NativeArray<JobHandle>(2, Allocator.Temp);
            jobHandles[0] = simulateParticlesJobHandle;
            jobHandles[1] = simulateColorsJobHandle;
            inputDeps = JobHandle.CombineDependencies(jobHandles);
        }
        m_UniqueParticlePropertiesEntries.Clear();
        return inputDeps;
    }
}
