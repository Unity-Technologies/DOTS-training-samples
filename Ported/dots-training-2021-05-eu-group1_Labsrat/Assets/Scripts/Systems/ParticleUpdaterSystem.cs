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
[UpdateAfter(typeof(ParticleSpawnerSystem))]
[AlwaysUpdateSystem]
public class ParticleUpdaterSystem : SystemBase
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

            var ecb = m_EcbSystem.CreateCommandBuffer();

            Entities
                .ForEach((Entity particle, ref Translation t, ref ParticleAge age, in ParticleLifetime life, in ParticleVelocity vel) => 
                {
                    if(age.Value > life.Value)
                    {
                        ecb.DestroyEntity(particle);
                    }
                    else
                    {
                        age.Value = age.Value + dt;
                        t.Value = t.Value + vel.Value * dt;
                    }

            }).Schedule();

        }
    }
}