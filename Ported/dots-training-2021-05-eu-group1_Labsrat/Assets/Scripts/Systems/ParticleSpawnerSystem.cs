using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

[UpdateInGroup(typeof(ChuChuRocketUpdateGroup))]
[AlwaysUpdateSystem]
public class ParticleSpawnerSystem : SystemBase
{
    EntityCommandBufferSystem m_EcbSystem;
    float TimeUntilNextSpawn;
    Random random;
    bool randomInitialized;

    protected override void OnCreate()
    {
        m_EcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            float dt = Time.DeltaTime;

            if (!randomInitialized)
            {
                random = Random.CreateFromIndex(gameConfig.RandomSeed ? (uint)System.DateTime.Now.Ticks : gameConfig.Seed ^ 2984576396);
                randomInitialized = true;
            }

            if(TimeUntilNextSpawn <= 0)
            {
                EntityCommandBuffer.ParallelWriter ecb = m_EcbSystem.CreateCommandBuffer().AsParallelWriter();

                Random r = random;

                Entities.ForEach((Entity mouseEntity, int entityInQueryIndex, in Mouse m, in Translation translation) =>
                {
                    Entity particle = ecb.Instantiate(entityInQueryIndex, gameConfig.ParticlePrefab);

                    ecb.SetComponent<Translation>(entityInQueryIndex, particle, new Translation() { Value = translation.Value });
                    ecb.SetComponent<Rotation>(entityInQueryIndex, particle, new Rotation() { Value = r.NextQuaternionRotation() }) ;
                    float3 vel = new float3(r.NextFloat(-1, 1), r.NextFloat(0, 1), r.NextFloat(-1, 1));

                    ecb.AddComponent<ParticleVelocity>(entityInQueryIndex, particle, new ParticleVelocity() { Value = vel });
                    ecb.AddComponent<ParticleLifetime>(entityInQueryIndex, particle, new ParticleLifetime() { Value = gameConfig.ParticleLifetime });
                    ecb.AddComponent<ParticleAge>(entityInQueryIndex, particle, new ParticleAge() { Value = 0 });
                    ecb.AddComponent<Scale>(entityInQueryIndex, particle, new Scale() { Value = r.NextFloat(.05f, .2f) });

                }).Schedule();

                TimeUntilNextSpawn = 1f / gameConfig.ParticleSpawnRate;

                m_EcbSystem.AddJobHandleForProducer(Dependency);
            }
            else
            {
                TimeUntilNextSpawn -= dt;
            }

        }
    }
}
