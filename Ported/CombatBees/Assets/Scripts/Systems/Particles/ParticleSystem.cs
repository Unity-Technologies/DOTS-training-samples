using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Systems.Particles
{
    [WithAll(typeof(BeeParticleComponent))]
    [BurstCompile]
    public partial struct MoveParticlesJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public float DeltaTime;

        public void Execute([ChunkIndexInQuery] int chunkIndex, in Entity entity, ref BeeParticleComponent particle,
            ref TransformAspect transform, ref PostTransformMatrix transformMatrix)
        {
            if (!particle.stuck)
            {
                particle.velocity += new float3(0, 1, 0) * (Field.gravity * DeltaTime);
                particle.position += particle.velocity * DeltaTime;

                if (System.Math.Abs(particle.position.x) > Field.size.x * .5f)
                {
                    particle.position.x = Field.size.x * .5f * math.sign(particle.position.x);
                    float splat = math.abs(particle.velocity.x * .3f) + 1f;
                    particle.size.y *= splat;
                    particle.size.z *= splat;
                    particle.stuck = true;
                }

                if (System.Math.Abs(particle.position.y) > Field.size.y * .5f)
                {
                    particle.position.y = Field.size.y * .5f * math.sign(particle.position.y);
                    float splat = math.abs(particle.velocity.y * .3f) + 1f;
                    particle.size.z *= splat;
                    particle.size.x *= splat;
                    particle.stuck = true;
                }

                if (System.Math.Abs(particle.position.z) > Field.size.z * .5f)
                {
                    particle.position.z = Field.size.z * .5f * math.sign(particle.position.z);
                    float splat = math.abs(particle.velocity.z * .3f) + 1f;
                    particle.size.x *= splat;
                    particle.size.y *= splat;
                    particle.stuck = true;
                }

                if (particle.stuck)
                {
                    transform.Rotation = quaternion.identity;
                    transformMatrix.Value = float4x4.Scale(particle.size);
                }
            }

            const float speedStretch = 0.25f;

            particle.color.w = particle.life;

            Ecb.SetComponent(chunkIndex, entity, new URPMaterialPropertyBaseColor
            {
                Value = particle.color
            });

            if (!particle.stuck)
            {
                var rotation = quaternion.identity;
                float3 scale = particle.size * particle.life;
                if (particle.type == ParticleType.Blood)
                {
                    rotation = quaternion.LookRotation(particle.velocity, new float3(0, 1, 0));
                    scale.z *= 1f + math.length(particle.velocity) * speedStretch;
                }

                transform.Rotation = rotation;
                transformMatrix.Value = float4x4.Scale(scale);
            }

            transform.Position = particle.position;
            
            particle.life -= DeltaTime / particle.lifeDuration;
            if (particle.life < 0f)
            {
                Ecb.DestroyEntity(chunkIndex, entity);
            }
        }
    }

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [BurstCompile]
    partial struct ParticleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var dt = SystemAPI.Time.DeltaTime;

             var moveParticlesJob = new MoveParticlesJob()
             {
                 DeltaTime = dt,
                 Ecb = ecb.AsParallelWriter(),
             };

            state.Dependency = moveParticlesJob.ScheduleParallel(state.Dependency);
        }
    }
}