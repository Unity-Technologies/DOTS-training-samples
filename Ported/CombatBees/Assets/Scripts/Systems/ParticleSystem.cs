using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Mathf = UnityEngine.Mathf;
using UnityMaterialPropertyBlock = UnityEngine.MaterialPropertyBlock;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(BeeMovementSystemFixed))]
public partial class ParticleSystemFixed : SystemBase
{
    EntityCommandBufferSystem beginFixedSimulationEntityCommandBufferSystem;
    EntityCommandBufferSystem endFixedSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        beginFixedSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginFixedStepSimulationEntityCommandBufferSystem>();
        endFixedSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var up = new float3(0, 1, 0);

        var beginEcb = beginFixedSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var endEcb = endFixedSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        // When components are stuck to a surface, velocity is removed which prevents them being considered for any movement jobs.

        // Update velocities
        var velocityJob = Entities
            .ForEach((Entity entity, ref Velocity velocity, in ParticleComponent particle) =>
            {
                velocity.Value += up * (PlayField.gravity * deltaTime);
            }).ScheduleParallel(Dependency);

        // Update positions from velocities
        var moveJob = Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Size size, in Velocity velocity, in ParticleComponent particle) =>
            {
                translation.Value += velocity.Value * deltaTime;

                if (Mathf.Abs(translation.Value.x) > PlayField.size.x * .5f)
                {
                    translation.Value.x = PlayField.size.x * .5f * Mathf.Sign(translation.Value.x);
                    float splat = Mathf.Abs(velocity.Value.x * .3f) + 1f;
                    size.Value.y *= splat;
                    size.Value.z *= splat;

                    endEcb.RemoveComponent<Velocity>(entityInQueryIndex, entity);
                }
                if (Mathf.Abs(translation.Value.y) > PlayField.size.y * .5f)
                {
                    translation.Value.y = PlayField.size.y * .5f * Mathf.Sign(translation.Value.y);
                    float splat = Mathf.Abs(velocity.Value.y * .3f) + 1f;
                    size.Value.z *= splat;
                    size.Value.x *= splat;

                    endEcb.RemoveComponent<Velocity>(entityInQueryIndex, entity);
                }
                if (Mathf.Abs(translation.Value.z) > PlayField.size.z * .5f)
                {
                    translation.Value.z = PlayField.size.z * .5f * Mathf.Sign(translation.Value.z);
                    float splat = Mathf.Abs(velocity.Value.z * .3f) + 1f;
                    size.Value.x *= splat;
                    size.Value.y *= splat;

                    endEcb.RemoveComponent<Velocity>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(velocityJob);

        var cleanupJob = Entities
            .WithNone<Velocity>()
            .ForEach((ref URPMaterialPropertyBaseColor color, ref Rotation rotation, ref NonUniformScale renderScale, in Size size) =>
            {
                rotation.Value = quaternion.identity;
                renderScale.Value = size.Value;

            }).ScheduleParallel(moveJob);


        // Can run in parallel to the other jobs
        var lifeJob = Entities
           .ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifetime, in ParticleComponent particle) =>
           {
               lifetime.Value -= deltaTime / lifetime.Duration;
               if (lifetime.Value < 0f)
               {
                   beginEcb.DestroyEntity(entityInQueryIndex, entity);
               }
           }).ScheduleParallel(Dependency);

        Dependency = Unity.Jobs.JobHandle.CombineDependencies(cleanupJob, lifeJob);

        beginFixedSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        endFixedSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class ParticleSystem : SystemBase
{
    public static void SpawnParticle(EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity entityPrefab, ref Random rand, float3 position, ParticleComponent.ParticleType type, float3 velocity, float velocityJitter = 6f, int count = 1)
    {
        // Processing each particle via the ECS makes a lot of sense, but creation costs may be an issue.Can pooling be built in?
        for (int i = 0; i < count; i++)
        {
            var entity = ecb.Instantiate(sortKey, entityPrefab);

            if (type == ParticleComponent.ParticleType.Blood)
            {
                float3 scale = rand.NextFloat(.1f, .2f);

                ecb.SetComponent(sortKey, entity, new Lifetime { Value = 1f, Duration = rand.NextFloat(3f, 5f) });
                ecb.SetComponent(sortKey, entity, new ParticleComponent { Type = type });
                ecb.SetComponent(sortKey, entity, new Translation { Value = position });
                ecb.SetComponent(sortKey, entity, new Velocity { Value = velocity + rand.NextFloat3Direction() * velocityJitter });
                ecb.SetComponent(sortKey, entity, new NonUniformScale { Value = scale.x });
                ecb.SetComponent(sortKey, entity, new Size { Value = scale.x });
                ecb.SetComponent(sortKey, entity, new URPMaterialPropertyBaseColor { Value = new float4(1, 0, 0, 1) });
            }
            else
            {
                float3 scale = rand.NextFloat(1f, 2f);

                ecb.SetComponent(sortKey, entity, new Lifetime { Value = 1f, Duration = rand.NextFloat(.25f, .5f) });
                ecb.SetComponent(sortKey, entity, new ParticleComponent { Type = type });
                ecb.SetComponent(sortKey, entity, new Translation { Value = position });
                ecb.SetComponent(sortKey, entity, new Velocity { Value = rand.NextFloat3Direction() * 5f });
                ecb.SetComponent(sortKey, entity, new NonUniformScale { Value = scale.x });
                ecb.SetComponent(sortKey, entity, new Size { Value = scale.x });
                ecb.SetComponent(sortKey, entity, new URPMaterialPropertyBaseColor { Value = new float4(1, 1, 1, 1) });
            }
        }
    }

    protected override void OnUpdate()
    {
        // Update for rendering for with and without velocity
        Entities
           .ForEach((ref URPMaterialPropertyBaseColor color, in Lifetime lifetime, in ParticleComponent particle) =>
           {
               color.Value.w = lifetime.Value;

           }).ScheduleParallel();

        Entities
          .ForEach((ref Rotation rotation, ref NonUniformScale renderScale, ref URPMaterialPropertyBaseColor color, in Size size, in Velocity velocity, in Lifetime lifetime, in ParticleComponent particle) =>
          {
              renderScale.Value = size.Value * lifetime.Value;
              if (particle.Type == ParticleComponent.ParticleType.Blood)
              {
                  rotation.Value = quaternion.LookRotation(velocity.Value, new float3(0, 1, 0));
                  renderScale.Value.z *= 1f + math.length(velocity.Value) * /*speedStretch*/ 0.25f;
              }

              color.Value.w = lifetime.Value;

          }).ScheduleParallel();
    }
}
