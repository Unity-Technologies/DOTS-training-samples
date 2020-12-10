using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BeeSpawnerSystem))]
[UpdateAfter(typeof(ResourceSpawnerSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class ParticleManagerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var particleParams = GetSingleton<ParticleParamsAuthoring>();
        var field = GetSingleton<FieldAuthoring>();

        float deltaTime = Time.fixedDeltaTime;

        /*
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithNone<Stuck>()
            .ForEach((Entity particleEntity, ref Velocity velocity, ref Translation pos,
                        ref NonUniformScale scale) =>
            {
                velocity.vel += math.up() * (field.gravity * deltaTime);
                pos.Value += velocity.vel * deltaTime;

                float splat;
                if(math.abs(pos.Value.x) > field.size.x * .5f)
                {
                    pos.Value.x = field.size.x * .5f * math.sign(pos.Value.x);
                    splat = math.abs(pos.Value.x * .3f) + 1f;
                    scale.Value.y *= splat;
                    scale.Value.z *= splat;
                    ecb.AddComponent<Stuck>(particleEntity);
                }

                if (math.abs(pos.Value.y) > field.size.y * .5f)
                {
                    pos.Value.y = field.size.y * .5f * math.sign(pos.Value.y);
                    splat = math.abs(pos.Value.y * .3f) + 1f;
                    scale.Value.x *= splat;
                    scale.Value.z *= splat;
                    ecb.AddComponent<Stuck>(particleEntity);
                }

                if (math.abs(pos.Value.z) > field.size.z * .5f)
                {
                    pos.Value.z = field.size.z * .5f * math.sign(pos.Value.z);
                    splat = math.abs(pos.Value.z * .3f) + 1f;
                    scale.Value.x *= splat;
                    scale.Value.y *= splat;
                    ecb.AddComponent<Stuck>(particleEntity);
                }

            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
        */

        /* --------------------------------------------------------------------------------- */

        /*
        var ecb1 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .ForEach((Entity particleEntity, ref Life life, in LifeDuration lifeDuration) =>
            {
                life.vel -= deltaTime / lifeDuration.vel;
                if(life.vel < 0f)
                {
                    ecb1.DestroyEntity(particleEntity);
                }

            }).Run();
        ecb1.Playback(EntityManager);
        ecb1.Dispose();

        Entities
            .ForEach((Entity particleEntity, ref LocalToWorld localToWorld, ref URPMaterialPropertyBaseColor baseColor, 
                        in ParticleType particleType, in NonUniformScale particleScale, in Life life,
                        in Velocity velocity, in Translation pos) =>
            {
                if(HasComponent<Stuck>(particleEntity) == false)
                {
                    quaternion rotation = quaternion.identity;
                    float3 scale = particleScale.Value * life.vel;
                    if(particleType.type == ParticleType.Type.Blood)
                    {
                        rotation = quaternion.LookRotation(velocity.vel, math.up());
                        scale.z *= 1f + math.length(velocity.vel) * particleParams.speedStretch;
                    }
                    localToWorld.Value = float4x4.TRS(pos.Value, rotation, scale);
                }

                baseColor.Value.w = life.vel;
            }).Run();
        */



    }
}

