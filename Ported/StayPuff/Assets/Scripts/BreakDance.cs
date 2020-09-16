using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class BreakDance : SystemBase
{
    EndFixedStepSimulationEntityCommandBufferSystem fixedStepSimulationCommandBufferSystem;

    protected override void OnCreate()
    {
        fixedStepSimulationCommandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter ecb = fixedStepSimulationCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        ComponentDataFromEntity<Translation> entityToTranslation = GetComponentDataFromEntity<Translation>();

        Entities
            .WithReadOnly(entityToTranslation)
            .ForEach((Entity entity, int entityInQueryIndex, ref PhysicsJoint joint, in PhysicsConstrainedBodyPair bodyPair, in JointBreakDistance breakDistance) =>
            {
                float3 positionA = entityToTranslation[bodyPair.EntityA].Value;
                float3 positionB = entityToTranslation[bodyPair.EntityB].Value;

                if(math.length(positionA - positionB) > breakDistance.Value)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }

            }).ScheduleParallel();

            fixedStepSimulationCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
}