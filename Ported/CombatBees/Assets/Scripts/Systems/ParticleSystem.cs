using Unity.Burst;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct ParticleSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var timeData = state.WorldUnmanaged.Time;
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var particleJob = new ParticleJob()
        {
            ecb = ecb.AsParallelWriter(),
            deltaTime = timeData.DeltaTime,
            config = config
        };
        particleJob.ScheduleParallel();

        /*foreach(var (particle, transform, entity) in SystemAPI.Query<RefRW<Particle>, TransformAspect>().WithEntityAccess())
        {
            particle.ValueRW.life -= timeData.DeltaTime / particle.ValueRW.lifeTime;
            if (particle.ValueRW.life <= 0f)
            {
                ecb.DestroyEntity(entity);
                continue;
            }
            transform.LocalScale = particle.ValueRW.size * particle.ValueRW.life;
            
            if (!particle.ValueRW.stuck)
            {
                particle.ValueRW.velocity += config.gravity * timeData.DeltaTime;
                transform.TranslateWorld(particle.ValueRW.velocity * timeData.DeltaTime);
                //transform.LookAt(transform.WorldPosition + particle.ValueRW.velocity);
                
                if (math.abs(transform.WorldPosition.x) > halfFieldSize.x)
                {
                    var worldX = config.fieldSize.x * .5f * math.sign(transform.WorldPosition.x);
                    transform.WorldPosition = new float3(worldX, transform.WorldPosition.y, transform.WorldPosition.z);
                    var splat = math.abs(particle.ValueRW.velocity.x * .3f) + 1f;
                    particle.ValueRW.stuck = true;
                    var size = particle.ValueRO.size;
                    ecb.SetComponent(entity, new PostTransformScale()
                    {
                        Value = float3x3.Scale(size, size * splat, size * splat)
                    });
                }
                else if (math.abs(transform.WorldPosition.y) > halfFieldSize.y)
                {
                    var worldY = config.fieldSize.y * .5f * math.sign(transform.WorldPosition.y);
                    transform.WorldPosition = new float3(transform.WorldPosition.x, worldY, transform.WorldPosition.z);
                    var splat = math.abs(particle.ValueRW.velocity.y * .3f) + 1f;
                    particle.ValueRW.stuck = true;
                    var size = particle.ValueRO.size;
                    ecb.SetComponent(entity, new PostTransformScale()
                    {
                        Value = float3x3.Scale(size * splat, size, size * splat)
                    });
                }
                else if (math.abs(transform.WorldPosition.z) > halfFieldSize.z)
                {
                    var worldZ = config.fieldSize.z * .5f * math.sign(transform.WorldPosition.z);
                    transform.WorldPosition = new float3(transform.WorldPosition.x, transform.WorldPosition.y, worldZ);
                    var splat = math.abs(particle.ValueRW.velocity.z * .3f) + 1f;
                    particle.ValueRW.stuck = true;
                    var size = particle.ValueRO.size;
                    ecb.SetComponent(entity, new PostTransformScale()
                    {
                        Value = float3x3.Scale(size * splat, size * splat, size)
                    });
                }
            }
        }*/
    }
}

[BurstCompile]
public partial struct ParticleJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;
    public float deltaTime;
    public Config config;

    private void Execute([ChunkIndexInQuery] int chunkIndex, ref ParticleAspect particle)
    {
        var halfFieldSize = config.fieldSize * .5f;
        particle.Life -= deltaTime / particle.LifeTime;
        if (particle.Life <= 0f)
        {
            ecb.DestroyEntity(chunkIndex, particle.self);
            return;
        }
        particle.LocalScale = math.clamp(particle.Size * particle.Life, 0f, 1f);
        //Debug.Log($"stuck {particle.Stuck}, life {particle.Life}");
        if (!particle.Stuck)
        {
            particle.Velocity += config.gravity * deltaTime;
            particle.WorldPosition += particle.Velocity * deltaTime;
            
            if (math.abs(particle.WorldPosition.x) > halfFieldSize.x)
            {
                var worldX = config.fieldSize.x * .5f * math.sign(particle.WorldPosition.x);
                particle.WorldPosition = new float3(worldX, particle.WorldPosition.y, particle.WorldPosition.z);
                var splat = math.abs(particle.Velocity.x * .3f) + 1f;
                particle.Stuck = true;
                var size = particle.Size;
                ecb.SetComponent(chunkIndex, particle.self, new PostTransformScale()
                {
                    Value = float3x3.Scale(size, size * splat, size * splat)
                });
            }
            else if (math.abs(particle.WorldPosition.y) > halfFieldSize.y)
            {
                var worldY = config.fieldSize.y * .5f * math.sign(particle.WorldPosition.y);
                particle.WorldPosition = new float3(particle.WorldPosition.x, worldY, particle.WorldPosition.z);
                var splat = math.abs(particle.Velocity.y * .3f) + 1f;
                particle.Stuck = true;
                var size = particle.Size;
                ecb.SetComponent(chunkIndex, particle.self, new PostTransformScale()
                {
                    Value = float3x3.Scale(size * splat, size, size * splat)
                });
            }
            else if (math.abs(particle.WorldPosition.z) > halfFieldSize.z)
            {
                var worldZ = config.fieldSize.z * .5f * math.sign(particle.WorldPosition.z);
                particle.WorldPosition = new float3(particle.WorldPosition.x, particle.WorldPosition.y, worldZ);
                var splat = math.abs(particle.Velocity.z * .3f) + 1f;
                particle.Stuck = true;
                var size = particle.Size;
                ecb.SetComponent(chunkIndex, particle.self, new PostTransformScale()
                {
                    Value = float3x3.Scale(size * splat, size * splat, size)
                });
            }
        }
    }
}