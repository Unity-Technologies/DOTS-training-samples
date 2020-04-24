using System;
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
    
    private struct RockReserveJob : IJob
    {
        public NativeQueue<RockReserveRequest> RequestQueue;
        public NativeHashMap<Entity, RockReserveRequest> RequestMap;
        public EntityCommandBuffer ECB;
            
        public void Execute( /*int index*/)
        {
            while (RequestQueue.TryDequeue(out var request))
            {
                if (!RequestMap.TryAdd(request.rockRef, request))
                {
                    //tie-breaker
                    var currentRequest = RequestMap[request.rockRef];
                    RequestMap[request.armRef] = request.armPos.x < currentRequest.armPos.x ? request : currentRequest;
                }
            }

            //todo "jobs can only create temp memory". Despite there being a "TempJob" memory. Why?
            var requestValues = RequestMap.GetValueArray(Allocator.Temp);

            for (int i = 0; i < requestValues.Length; i++)
            {
                var request = requestValues[i];
                ECB.AddComponent<RockReservedTag>(request.rockRef);

                ECB.AddComponent(request.armRef, new ArmReservedRock
                {
                    value = request.rockRef
                });
            }

            requestValues.Dispose();
        }
    };

    protected override void OnCreate()
    {
        availableRocksQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] {ComponentType.ReadOnly<RockRadiusComponentData>(), ComponentType.ReadOnly<RockTag>()},
            None = new[] {ComponentType.ReadWrite<RockReservedTag>(),ComponentType.ReadOnly<RockGrabbedTag>()}
        });

        beginSimEcbSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float t = (float) Time.ElapsedTime;
        float dt = (float) Time.DeltaTime;
        
        //1.8^2
        float reachDistSq = 3.24f;
        float reachDuration = 1.5f;
        

        var reserveQueue = new NativeQueue<RockReserveRequest>(Allocator.TempJob);
        //todo query for armLength and use for capacity parameter
        var RequestMap = new NativeHashMap<Entity, RockReserveRequest>(100, Allocator.TempJob);

        var reserveParallelWriter = reserveQueue.AsParallelWriter();

        //availableRocksQuery.AddDependency(Dependency); // TODO: ask Aria is this would remove the need to combine the job handles below
        var availableRockEntities =
            availableRocksQuery.ToEntityArrayAsync(Allocator.TempJob, out var getAvailableRocksJob);
        var ecb = beginSimEcbSystem.CreateCommandBuffer().ToConcurrent();
        var reserveECB = beginSimEcbSystem.CreateCommandBuffer();

        var rockReserveJobInputDeps = JobHandle.CombineDependencies(Dependency, getAvailableRocksJob);

        rockReserveJobInputDeps.Complete();


        var rockRequestJob = Entities
            .WithReadOnly(availableRockEntities)
            .WithNone<ArmReservedRock,ArmGrabbedTag>()
            .WithName("RockRequestJob")
            .WithDeallocateOnJobCompletion(availableRockEntities)
            .ForEach((Entity entity, int entityInQueryIndex, in ArmAnchorPos anchorPos, in ArmGrabTimer grabT) =>
            {
                if (grabT <= 0.0f)
                {
                    var minDistSquared = float.MaxValue;
                    int minIndex = -1;

                    for (int i = 0; i < availableRockEntities.Length; i++)
                    {
                        var rock = availableRockEntities[i];
                        var rockPos = GetComponent<Translation>(rock).Value;
                        float distSq = math.distancesq(rockPos, anchorPos);
                        if (distSq < minDistSquared)
                        {
                            minDistSquared = distSq;
                            minIndex = i;
                        }
                    }


                    if (minIndex > -1 && minDistSquared < reachDistSq)
                    {
                        reserveParallelWriter.Enqueue(new RockReserveRequest
                        {
                            armPos = anchorPos,
                            armRef = entity,
                            rockRef = availableRockEntities[minIndex]
                        });
                    }
                }

            }).ScheduleParallel(rockReserveJobInputDeps);

        

        var rockReserveJob = new RockReserveJob
        {
            RequestQueue = reserveQueue,
            RequestMap = RequestMap,
            ECB = reserveECB
        }.Schedule(rockRequestJob);

        reserveQueue.Dispose(rockReserveJob);
        RequestMap.Dispose(rockReserveJob);

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

        var targetBackJob = Entities
            .WithName("ArmLerpBackJob")
            .WithNone<ArmReservedRock>()
            .ForEach((ref ArmGrabTimer grabT) =>
            {
                grabT -= dt / reachDuration;
                grabT = math.clamp(grabT, 0f, 1f);
            }).ScheduleParallel(JobHandle.CombineDependencies(rockRequestJob,idleJob));
        
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

                grabT += dt / reachDuration;
                grabT = math.clamp(grabT, 0f, 1f);
            }).ScheduleParallel(targetBackJob);

        JobHandle grabInputDeps = JobHandle.CombineDependencies(rockRequestJob, targetJob);

        var grabEcb = beginSimEcbSystem.CreateCommandBuffer().ToConcurrent();
        var grabJob = Entities
            .WithName("ArmGrabJob")
            .ForEach((
                int entityInQueryIndex,
                Entity armEntity,
                ref ArmReservedRock reservedRock,
                in ArmGrabTimer grabT,
                in ArmAnchorPos anchorPos,
                in Wrist wrist
            ) =>
            {
                float3 wristToRock = GetComponent<Translation>(reservedRock).Value -
                                     (GetComponent<Translation>(wrist).Value);
                
                float3 anchorToRock = GetComponent<Translation>(reservedRock).Value - anchorPos;
                
                if (grabT >= 1.0f)
                {
                    if (!HasComponent<Parent>(reservedRock.value))
                    {
                        Parent wristParent = new Parent
                        {
                            Value = wrist
                        };
                        
                        grabEcb.SetComponent(entityInQueryIndex, reservedRock, new Translation
                        {
                            Value = wristToRock
                        });

                        grabEcb.AddComponent(entityInQueryIndex, reservedRock, wristParent);
                        grabEcb.AddComponent(entityInQueryIndex, reservedRock, new LocalToParent());
                        grabEcb.AddComponent(entityInQueryIndex, reservedRock, new RockGrabbedTag());
                        
                        grabEcb.AddComponent<ArmGrabbedTag>(entityInQueryIndex, armEntity);
                        grabEcb.RemoveComponent<ArmReservedRock>(entityInQueryIndex, armEntity);
                    }
                }
                else if (math.lengthsq(anchorToRock) >  reachDistSq)
                {
                    grabEcb.RemoveComponent<RockReservedTag>(entityInQueryIndex,reservedRock);
                    grabEcb.RemoveComponent<ArmReservedRock>(entityInQueryIndex,armEntity);
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
        Dependency = JobHandle.CombineDependencies(rockRequestJob, armIkJob);
    }
}