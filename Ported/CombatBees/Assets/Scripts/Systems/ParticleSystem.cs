using Unity.Burst;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

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
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var halfFieldSize = config.fieldSize * .5f;

        foreach(var (particle, colorProp, transform, entity) in SystemAPI.Query<RefRW<Particle>, RefRW<URPMaterialPropertyBaseColor>, TransformAspect>().WithEntityAccess())
        {
            particle.ValueRW.life -= timeData.DeltaTime / particle.ValueRW.lifeTime;
            if (particle.ValueRW.life <= 0f)
            {
                ecb.DestroyEntity(entity);
                continue;
            }
            // var color = colorProp.ValueRW.Value;
            // color.w = particle.ValueRW.life;
            // colorProp.ValueRW.Value = color;
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
        }
    }
}