using System.Runtime.CompilerServices;
using Components;
using Helpers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Systems.Particles
{
    public static class ParticleBuilder
    {
        public static BeeParticleComponent Create(float3 position, ParticleType type, float3 velocity,
            float velocityJitter, Random rand)
        {
            var particle = new BeeParticleComponent
            {
                type = type,
                position = position,
                life = 1f
            };

            if (type == ParticleType.Blood)
            {
                var randVelocity = rand.NextInsideSphere();
                particle.velocity = velocity + randVelocity * velocityJitter;
                particle.lifeDuration = rand.Range(3f, 5f);
                particle.size = new float3(1, 1, 1) * rand.Range(.1f, .2f);
                particle.color = new float4(0.5f + rand.NextFloat() * 0.5f, 0, 0, 1);
            }
            else if (type == ParticleType.SpawnFlash)
            {
                var randVelocity = rand.NextInsideSphere();
                particle.velocity = randVelocity * 5f;
                particle.lifeDuration = rand.Range(.25f, .5f);
                particle.size = new float3(1, 1, 1) * rand.Range(1f, 2f);
                particle.color = new float4(1, 1, 1, 1);
            }

            return particle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpawnParticleEntity(EntityCommandBuffer.ParallelWriter writer, int chunkIndex, uint randomSeed,
            Entity prefab, float3 position, ParticleType type, float3 velocity,
            float velocityJitter, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                var random = Random.CreateFromIndex(randomSeed + (uint)i);
                var particleComponent = Create(position, type, velocity, velocityJitter, random);

                var particleEntity = writer.Instantiate(chunkIndex, prefab);

                var uniformScaleTransform = new UniformScaleTransform
                {
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1f
                };
                writer.SetComponent(chunkIndex, particleEntity, new LocalToWorldTransform
                {
                    Value = uniformScaleTransform
                });
                
                writer.SetComponent(chunkIndex, particleEntity, new URPMaterialPropertyBaseColor
                {
                    Value = particleComponent.color
                });
                
                writer.AddComponent(chunkIndex, particleEntity, new PostTransformMatrix());
                writer.AddComponent(chunkIndex, particleEntity, particleComponent);
            }
        }
    }
}