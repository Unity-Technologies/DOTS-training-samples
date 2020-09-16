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
    EntityQuery TornadoListQuery;
    EndFixedStepSimulationEntityCommandBufferSystem fixedStepSimulationCommandBufferSystem;

    protected override void OnCreate()
    {
        TornadoListQuery = GetEntityQuery(typeof(TornadoForceData), ComponentType.ReadOnly<Translation>());
        fixedStepSimulationCommandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        int numTornados = TornadoListQuery.CalculateEntityCount();
        NativeList<float4> packedTornadoValues = new NativeList<float4>(numTornados, Allocator.TempJob);
        NativeList<float4>.ParallelWriter tornadoPositionWriter = packedTornadoValues.AsParallelWriter();
        
        EntityCommandBuffer.ParallelWriter ecb = fixedStepSimulationCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        ComponentDataFromEntity<Translation> entityToTranslation = GetComponentDataFromEntity<Translation>();

        Entities
            .ForEach((in TornadoForceData forcedata, in Translation tornadopos) =>
            {
                float4 packedValue = new float4(tornadopos.Value, forcedata.tornadoBreakDist);      
                tornadoPositionWriter.AddNoResize(packedValue);
            }).ScheduleParallel();

        Entities
            .WithReadOnly(packedTornadoValues)
            .WithReadOnly(entityToTranslation)
            .WithDisposeOnCompletion(packedTornadoValues)
            .ForEach((Entity entity, int entityInQueryIndex, ref PhysicsJoint joint, in PhysicsConstrainedBodyPair bodyPair) =>
            {
                float3 positionA = entityToTranslation[bodyPair.EntityA].Value;
                //float3 positionB = entityToTranslation[bodyPair.EntityB].Value;

                for (int i = 0; i < packedTornadoValues.Length; i++)
                {
                    float4 packedData = packedTornadoValues[i];
                    float3 tornadoPos = new float3(packedData.x, packedData.y, packedData.z);
                    float threshold = packedData.w;

                    float distance = math.length(tornadoPos - positionA);
                    if (distance < threshold)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                        break;
                    }
                    //distance = math.length(tornado - positionB);
                    //if (distance < threshold)
                    //{
                    //    ecb.DestroyEntity(sortKey, entity);
                    //    break;
                    //}
                }
            }).ScheduleParallel();

            fixedStepSimulationCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
}