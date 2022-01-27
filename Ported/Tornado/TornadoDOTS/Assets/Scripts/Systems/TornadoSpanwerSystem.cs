using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

/// <summary>
/// Intentionally spawning the particles in the floor.
/// In the demo, the cubes spawn all over the place,
/// but still, these particles will be attracted to the tornado. 
/// </summary>
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class TornadoSpanwerSystem : SystemBase
{
    const int k_Padding = 4;
    EntityCommandBufferSystem m_EcbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_EcbSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_EcbSystem.CreateCommandBuffer().AsParallelWriter();
        var random = new Random((uint)DateTime.Now.Millisecond+1);
        var floor = GetSingletonEntity<Floor>();
        var scale = GetComponent<NonUniformScale>(floor).Value*k_Padding;
        
        Entities
            .ForEach((Entity entity, int entityInQueryIndex, in TornadoSpawner spawner) =>
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
                
                int particleCount = spawner.particleCount;
                for (int i = 0; i < particleCount; i++)
                {
                    var particleInstance = ecb.Instantiate(entityInQueryIndex, spawner.particlePrefab);

                    var randomXZPos = random.NextFloat3(scale*-1 ,scale);
                    var translation = new Translation {Value = new float3(randomXZPos.x, 0, randomXZPos.z)};
                    var particleProps = new TornadoParticle
                        {SpinRate = random.NextFloat(1, 60), UpwardSpeed = random.NextFloat(1, 25), RadiusMult = random.NextFloat(1)};
                        
                    ecb.SetComponent(entityInQueryIndex, particleInstance, translation);
                    ecb.SetComponent(entityInQueryIndex, particleInstance, particleProps);
                    float baseColor = random.NextFloat(0.3f, 0.7f); 
                    ecb.SetComponent(entityInQueryIndex, particleInstance, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4(baseColor,baseColor,baseColor,1)
                    });
                }
            })
            .ScheduleParallel();

        m_EcbSystem.AddJobHandleForProducer(Dependency);
    }
}
