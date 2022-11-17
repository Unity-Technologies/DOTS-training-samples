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

        [BurstCompile]
        public void Execute([ChunkIndexInQuery] int chunkIndex, in Entity entity, ref BeeParticleComponent particle,
            ref TransformAspect transform, ref PostTransformMatrix transformMatrix)
        {
            if (!particle.stuck)
            {
                particle.velocity += new float3(0, 1, 0) * (Field.gravity * DeltaTime);
                particle.position += particle.velocity * DeltaTime;

                if (math.abs(particle.position.x) > Field.size.x * .5f)
                {
                    particle.position.x = Field.size.x * .5f * math.sign(particle.position.x);
                    float splat = math.abs(particle.velocity.x * .3f) + 1f;
                    particle.size.y *= splat;
                    particle.size.z *= splat;
                    particle.stuck = true;
                }

                if (math.abs(particle.position.y) > Field.size.y * .5f)
                {
                    particle.position.y = Field.size.y * .5f * math.sign(particle.position.y);
                    float splat = math.abs(particle.velocity.y * .3f) + 1f;
                    particle.size.z *= splat;
                    particle.size.x *= splat;
                    particle.stuck = true;
                }

                if (math.abs(particle.position.z) > Field.size.z * .5f)
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
            
            var scale = particle.size * particle.life;
            
            if (!particle.stuck)
            {
                if (particle.type == ParticleType.Blood)
                {
                    var rotation = quaternion.LookRotation(particle.velocity, new float3(0, 1, 0));
                    
                    const float speedStretch = 0.25f;
                    scale.z *= 1f + math.lengthsq(particle.velocity) * speedStretch;
                    transform.Rotation = rotation;
                }
            }
            
            scale.x = math.min(scale.x, 2);
            scale.y = math.min(scale.y, 2);
            scale.z = math.min(scale.z, 2);

            transformMatrix.Value = float4x4.Scale(scale);
            transform.Position = particle.position;
            
            particle.life -= DeltaTime / particle.lifeDuration;
            if (particle.life < 0f)
            {
                Ecb.DestroyEntity(chunkIndex, entity);
            }
        }
    }

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