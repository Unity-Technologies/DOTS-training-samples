using Components;
using Helpers;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems.Particles
{
    public static class ParticleBuilder
    {
        public static BeeParticleComponent Create(float3 position, ParticleType type, float3 velocity,
            float velocityJitter, Random rand)
        {
            BeeParticleComponent particle = new BeeParticleComponent();

            particle.type = type;
            particle.position = position;
            particle.life = 1f;

            if (type == ParticleType.Blood)
            {
                var randVelocity = rand.NextInsideSphere();
                particle.velocity =
                    velocity + new float3(randVelocity.x, randVelocity.y, randVelocity.z) * velocityJitter;
                particle.lifeDuration = rand.Range(3f, 5f);
                particle.size = Vector3.one * rand.Range(.1f, .2f);

                particle.color = new float4(0.5f + rand.NextFloat() * 0.5f, 0,0,1);
            }
            else if (type == ParticleType.SpawnFlash)
            {
                var randVelocity = rand.NextInsideSphere();
                particle.velocity = randVelocity * 5f;
                particle.lifeDuration = rand.Range(.25f, .5f);
                particle.size = Vector3.one * rand.Range(1f, 2f); 
                particle.color = new float4(1,1,1,1);
            }

            return particle;
        }
    }
}