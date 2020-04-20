﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.SceneManagement;
 using UnityEngine;

 [UpdateBefore(typeof(ArmRenderUpdateSystem))]
public class ArmSystem : SystemBase
{
    private EntityQuery availableRocksQuery;
    private BeginSimulationEntityCommandBufferSystem beginSimEcbSystem;

    protected override void OnCreate()
    {
        availableRocksQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] {ComponentType.ReadOnly<RockRadiusComponentData>()},
            None = new[] {ComponentType.ReadWrite<RockReservedTag>()}
        });

        beginSimEcbSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float t = (float) Time.ElapsedTime;
        float dt = (float) Time.DeltaTime;

        //availableRocksQuery.AddDependency(Dependency); // TODO: ask Aria is this would remove the need to combine the job handles below
        var availableRockEntities =
            availableRocksQuery.ToEntityArrayAsync(Allocator.TempJob, out var getAvailableRocksJob);
        var ecb = beginSimEcbSystem.CreateCommandBuffer().ToConcurrent();
        
        var rockReserveJobInputDeps = JobHandle.CombineDependencies(Dependency, getAvailableRocksJob);
        
        var rockReserveJob = Entities
            .WithReadOnly(availableRockEntities)
            .WithNone<ArmReservedRock>()
            .WithName("RockReserverJob")
            .WithDeallocateOnJobCompletion(availableRockEntities)
            .ForEach((Entity entity, int entityInQueryIndex, in ArmAnchorPos anchorPos) =>
            {
                for (int i = 0; i < availableRockEntities.Length; i++)
                {
                    var rock = availableRockEntities[i];
                    var rockPos = GetComponent<LocalToWorld>(rock).Position;
                    float distSq = math.distancesq(rockPos, anchorPos);
                    float reachSq = 2.0f * 2.0f;
                    if (distSq < reachSq)
                    {
                        ecb.AddComponent(entityInQueryIndex, entity, new ArmReservedRock
                        {
                            value = rock
                        });
                        ecb.AddComponent(entityInQueryIndex, rock, new RockReservedTag());
                    }
                }
            }).ScheduleParallel(rockReserveJobInputDeps);

        beginSimEcbSystem.AddJobHandleForProducer(rockReserveJob);

        var idleJob = Entities
            .WithName("ArmIdleJob")
            .ForEach((ref ArmIdleTarget armIdleTarget,
            in ArmAnchorPos anchorPos) =>
        {
            float time = t + anchorPos.value.x + anchorPos.value.y + anchorPos.value.z;

            armIdleTarget = anchorPos +
                            new float3(math.sin(time) * .35f, 1f + math.cos(time * 1.618f) * .5f, 1.5f);
        }).ScheduleParallel(Dependency);

        var targetJob = Entities
            .WithName("ArmTargetJob")
            .ForEach((ref ArmGrabTarget grabTarget,
            ref ArmGrabTimer grabT,
            ref ArmLastRockRecord lastRockRecord,
            in ArmAnchorPos anchorPos,
            in ArmBasesUp armUp,
            in ArmReservedRock reservedRock) =>
        {
            float3 rockPos = GetComponent<LocalToWorld>(reservedRock).Position;
            float rockSize = GetComponent<RockRadiusComponentData>(reservedRock);

            lastRockRecord.pos = rockPos;
            lastRockRecord.size = rockSize;

            float3 delta = rockPos - anchorPos;
            Debug.Assert(math.lengthsq(delta) > 0, "rock and anchor can not be in the same place");
            float3 flatDelta = delta;
            flatDelta.y = 0f;
            flatDelta = math.normalize(flatDelta);

            grabTarget = rockPos + armUp.value * rockSize * .5f -
                         flatDelta * rockSize * .5f;

            grabT = 1.02f * math.sin(0.2f * t);
            grabT = math.clamp(grabT, 0f, 1f);
        }).ScheduleParallel(idleJob);
        
        JobHandle grabInputDeps = JobHandle.CombineDependencies(rockReserveJob,targetJob);

        var grabEcb = beginSimEcbSystem.CreateCommandBuffer().ToConcurrent();
        var grabJob = Entities
            .WithName("ArmGrabJob")
            .ForEach((
            int entityInQueryIndex,
            Entity armEntity,
            ref ArmReservedRock reservedRock,
            in ArmGrabTimer grabT,
            in Wrist wrist
        ) =>
        {
            if (grabT >= 1.0f)
            {
                if (!HasComponent<Parent>(reservedRock.value))
                {
                    Parent wristParent = new Parent
                    {
                        Value = wrist
                    };
                    
                    float3 wristToRock = GetComponent<Translation>(reservedRock).Value -
                                         (GetComponent<Translation>(wrist).Value);

                    
                    grabEcb.SetComponent(entityInQueryIndex, reservedRock, new Translation
                    {
                        Value = wristToRock
                    });
                    
                    grabEcb.AddComponent(entityInQueryIndex, reservedRock, wristParent);
                    grabEcb.AddComponent(entityInQueryIndex, reservedRock, new LocalToParent());
                    grabEcb.AddComponent(entityInQueryIndex, reservedRock, new DebugRockGrabbedTag());
                }
            }
            
        }).ScheduleParallel(grabInputDeps);
        beginSimEcbSystem.AddJobHandleForProducer(grabJob);
        
        //JobHandle lerpInputJobs = JobHandle.CombineDependencies(idleJob, targetJob,grabJob);

        var ikTargetJob = Entities
            .WithName("ArmIKLerpJob")
            .ForEach((ref ArmIKTarget armIKTarget,
            in ArmIdleTarget armIdleTarget,
            in ArmGrabTarget armGrabTarget,
            in ArmGrabTimer grabT) =>
        {
            armIKTarget.value = math.lerp(armIdleTarget, armGrabTarget, grabT);
        }).ScheduleParallel(grabJob);

        var TranslationFromEntity = GetComponentDataFromEntity<Translation>(false);
        var armIkJob = Entities
            .WithName("armIKJob")
            .WithNativeDisableParallelForRestriction(TranslationFromEntity)
            .ForEach((
                int entityInQueryIndex,
                ref ArmBasesForward armForward,
                ref ArmBasisRight armRight,
                ref ArmBasesUp armUp,
                ref DynamicBuffer<ArmJointElementData> armJoints,
                ref Wrist wrist,
                in ArmIKTarget armTarget,
                in ArmAnchorPos armAnchorPos
            ) =>
            {
                FABRIK.Solve(armJoints.AsNativeArray().Reinterpret<float3>(), 1f, armAnchorPos.value, armTarget.value,
                    0.1f * armUp.value);

                //SetComponent(wrist,new Translation
                TranslationFromEntity[wrist] = new Translation
                {
                    Value = armJoints[armJoints.Length - 1].value
                }; 
                
                
                float3 transformRight = new float3(1, 0, 0);
                armForward.value =
                    math.normalize(armJoints[armJoints.Length - 1].value - armJoints[armJoints.Length - 2].value);
                armUp.value = math.normalize(math.cross(armForward.value, transformRight));
                armRight.value = math.cross(armUp.value, armForward.value);
            }).ScheduleParallel(ikTargetJob);

        // Last job becomes the new output dependency
        Dependency = JobHandle.CombineDependencies(rockReserveJob, armIkJob);
    }
}