using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using Unity.Rendering;

[UpdateInGroup(typeof(InitializationSystemGroup)), UpdateAfter(typeof(Sys_Random_Init))]
public class Sys_Particle_Init : JobComponentSystem
{
    struct Sys_Particle_InitJob : IJobChunk
    {
        [ReadOnly] public ArchetypeChunkEntityType EntityType;
        //These will use Random ONCE and don't affect logic, so it's okay if they are slightly predictable.
        public Random Rand;
        public EntityCommandBuffer.Concurrent ecb;
        
        [ReadOnly] public float PortionOfOriginalVelocity;
        [NativeDisableContainerSafetyRestriction]public ArchetypeChunkComponentType<C_Velocity> VelocityType;
        [NativeDisableContainerSafetyRestriction]public ArchetypeChunkComponentType<C_DeathTimer> TimerType;
        [NativeDisableContainerSafetyRestriction]public ArchetypeChunkComponentType<C_Size> SizeType;
        [ReadOnly] public float Spread;
        [ReadOnly] public float2 LifeSpanRange;
        [ReadOnly] public float2 SizeRange;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var Velocities = chunk.GetNativeArray(VelocityType);
            var Entities = chunk.GetNativeArray(EntityType);
            var Sizes = chunk.GetNativeArray(SizeType);
            var Timers = chunk.GetNativeArray(TimerType);

            for (int i = 0; i < chunk.Count; ++i)
            {
                var originalVel = Velocities[i].Value;
                var newVel = new C_Velocity()
                {
                    Value = Velocities[i].Value
                };
                newVel.Value = PortionOfOriginalVelocity * originalVel + Rand.NextFloat3Direction() * Spread;
                Velocities[i] = newVel;

                var timer = new C_DeathTimer()
                {
                    TimeRemaining = Rand.NextFloat(LifeSpanRange.x, LifeSpanRange.y)
                };
                Timers[i] = timer;
                
                var size = new C_Size()
                {
                    Value = Rand.NextFloat(SizeRange.x, SizeRange.y)
                };
                Sizes[i] = size;

                ecb.RemoveComponent(chunkIndex, Entities[i], typeof(Tag_Particle_Init));
            }
        }
    }

    private EntityCommandBufferSystem m_EntityCommandBufferSystem;

    private EntityQuery m_bloodQuery;
    private EntityQuery m_smokeQuery;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

        m_bloodQuery = GetEntityQuery(ComponentType.ReadOnly<C_Velocity>(), ComponentType.ReadOnly<RenderMesh>(), ComponentType.ReadOnly<Tag_Particle_Init>());
        m_smokeQuery = GetEntityQuery(ComponentType.ReadOnly<C_Velocity>(), ComponentType.ReadOnly<RenderMesh>(), ComponentType.ReadOnly<Tag_Particle_Init>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        m_bloodQuery.SetFilter(GameConstants.S.BloodRenderMesh);
        var bloodJob = new Sys_Particle_InitJob()
        {
            ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            EntityType = GetArchetypeChunkEntityType(),
            VelocityType = GetArchetypeChunkComponentType<C_Velocity>(false),
            TimerType = GetArchetypeChunkComponentType<C_DeathTimer>(false),
            SizeType = GetArchetypeChunkComponentType<C_Size>(false),
            Rand = BeeManager.S.Rand,
            PortionOfOriginalVelocity = 1.0f,
            Spread = 2f,
            LifeSpanRange = float2(3f, 5f),
            SizeRange = float2(.1f, .2f)
        }.Schedule(m_bloodQuery, inputDependencies);

        m_smokeQuery.SetFilter(GameConstants.S.SmokeRenderMesh);
        var smokeJob = new Sys_Particle_InitJob()
        {
            ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            EntityType = GetArchetypeChunkEntityType(),
            VelocityType = GetArchetypeChunkComponentType<C_Velocity>(false),
            TimerType = GetArchetypeChunkComponentType<C_DeathTimer>(false),
            SizeType = GetArchetypeChunkComponentType<C_Size>(false),
            Rand = BeeManager.S.Rand,
            PortionOfOriginalVelocity = 0f,
            Spread = 10f,
            LifeSpanRange = float2(.5f, 1f),
            SizeRange = float2(1f, 2f)
        }.Schedule(m_smokeQuery, inputDependencies);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(bloodJob);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(smokeJob);

        return JobHandle.CombineDependencies(bloodJob, smokeJob);
    }
}