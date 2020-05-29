using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(RenderUpdateSystem))]
public class ArmSetupSystem : SystemBase
{
    private EntityQuery m_availableRocksQuery;
    private BeginSimulationEntityCommandBufferSystem m_beginSimEcbSystem;
    
    [BurstCompile]
    private struct RockReserveJob : IJob
    {
        public NativeQueue<RockReserveRequest> requestQueue;
        public NativeHashMap<Entity, RockReserveRequest> requestMap;
        public EntityCommandBuffer ecb;

        public void Execute()
        {
            while (requestQueue.TryDequeue(out var request))
            {
                if (!requestMap.TryAdd(request.rockRef, request))
                {
                    //tie-breaker
                    var currentRequest = requestMap[request.rockRef];
                    requestMap[request.armRef] = request.armPos.x < currentRequest.armPos.x ? request : currentRequest;
                }
            }

            var requestValues = requestMap.GetValueArray(Allocator.Temp);

            for (int i = 0; i < requestValues.Length; i++)
            {
                var request = requestValues[i];
                ecb.AddComponent<RockReservedTag>(request.rockRef);

                ecb.AddComponent(request.armRef, new ArmReservedRock
                {
                    Value = request.rockRef
                });
            }

            requestValues.Dispose();
        }
    };

    protected override void OnCreate()
    {
        m_availableRocksQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] {ComponentType.ReadOnly<RockRadiusComponentData>()},
            None = new[] {ComponentType.ReadWrite<RockReservedTag>(), ComponentType.ReadOnly<RockGrabbedTag>()}
        });

        m_beginSimEcbSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float t = (float) Time.ElapsedTime;
        float dt = Time.DeltaTime;

        //1.8^2
        float reachDistSq = 3.24f;
        //float reachDuration = 1f;


        var reserveQueue = new NativeQueue<RockReserveRequest>(Allocator.TempJob);
        //todo query for number of arms in scene and use for capacity parameter
        var RequestMap = new NativeHashMap<Entity, RockReserveRequest>(100, Allocator.TempJob);

        var reserveParallelWriter = reserveQueue.AsParallelWriter();

        //m_availableRocksQuery.AddDependency(Dependency); // TODO: ask Aria if this would remove the need to combine the job handles below
        
        var availableRockEntities =
            m_availableRocksQuery.ToEntityArrayAsync(Allocator.TempJob, out var getAvailableRocksJob);
        
        var armQueueReservesInputDeps = JobHandle.CombineDependencies(Dependency, getAvailableRocksJob);
        
        //idle arms
        var armRequestJob = Entities
            .WithReadOnly(availableRockEntities)
            .WithNone<ArmReservedRock, ArmGrabbedTag>()
            .WithName("ArmRequestJob")
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
            }).ScheduleParallel(armQueueReservesInputDeps);

        
        var rockReserveECB = m_beginSimEcbSystem.CreateCommandBuffer();
        
        var rockReserveJob = new RockReserveJob
        {
            requestQueue = reserveQueue,
            requestMap = RequestMap,
            ecb = rockReserveECB
        }.Schedule(armRequestJob);

        reserveQueue.Dispose(rockReserveJob);
        RequestMap.Dispose(rockReserveJob);

        m_beginSimEcbSystem.AddJobHandleForProducer(rockReserveJob);
        
        
        //all arms
        var armIdleTargetJob = Entities
            .WithName("ArmIdleTargetJob")
            .ForEach((ref ArmIdleTarget armIdleTarget,
                in ArmAnchorPos anchorPos) =>
            {
                float time = t + anchorPos.value.x + anchorPos.value.y + anchorPos.value.z;

                armIdleTarget = anchorPos +
                                new float3(math.sin(time) * .35f, 1f + math.cos(time * 1.618f) * .5f, 1.5f);
            }).ScheduleParallel(Dependency);
        
        var childrenGroups = GetBufferFromEntity<Child>(true);
        
        //all arms with rock (grabbed, windup, and throwing)
        //todo can this be simplified using ArmReservedRock?
         var armRecordsJob = Entities
            .WithName("ArmRockRecordUpdateJob")
            .WithReadOnly(childrenGroups)
            .WithAll<ArmGrabbedTag>()
            .ForEach((
                ref ArmLastRockRecord lastRockRecord,
                in Wrist wrist) =>
            {
                
                /*due to the ECB playing back the GrabJob's action of adding a Parent component to the wrist, but before
                 the TransformSystem adds a child to the wrist.*/ 
                if (!childrenGroups.Exists(wrist))
                    return;

                //assumptiopn: a grabbed tag indicates that the wrist has exactly one child
                var rockChildren = childrenGroups[wrist];
                var rock = rockChildren[0].Value;

                float3 rockPos = GetComponent<LocalToWorld>(rock).Position;
                float rockSize = GetComponent<RockRadiusComponentData>(rock);

                lastRockRecord.pos = rockPos;
                lastRockRecord.size = rockSize;
            }).ScheduleParallel(armRequestJob);

         Dependency = JobHandle.CombineDependencies(armRecordsJob, armIdleTargetJob);
    }
}