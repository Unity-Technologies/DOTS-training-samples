using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(ArmIKSystem))]
public class CanSystem: SystemBase
{
    
    private struct CanReserveJob : IJob
    {
        public NativeQueue<CanReserveRequest> requestQueue;
        public NativeHashMap<Entity, CanReserveRequest> requestMap;
        public EntityCommandBuffer ecb;

        public void Execute()
        {
            while (requestQueue.TryDequeue(out var request))
            {
                if (!requestMap.TryAdd(request.canRef, request))
                {
                    //tie-breaker
                    var currentRequest = requestMap[request.canRef];
                    requestMap[request.armRef] = request.armPos.x < currentRequest.armPos.x ? request : currentRequest;
                }
            }

            var requestValues = requestMap.GetValueArray(Allocator.Temp);

            for (int i = 0; i < requestValues.Length; i++)
            {
                var request = requestValues[i];
                ecb.AddComponent<CanReservedTag>(request.canRef);

                ecb.AddComponent(request.armRef, new ArmReservedCan
                {
                    Value = request.canRef
                });
                ecb.AddComponent(request.rockRef, new RockReservedCan()
                {
                    Value = request.canRef
                });
                ecb.AddComponent(request.armRef, new ArmWindupTimer
                {
                    Value = -1f,
                });
                ecb.AddComponent(request.armRef, new ArmWindupTarget());
                ecb.AddComponent(request.armRef, new ArmLastThrowRecord());
            }

            requestValues.Dispose();
        }
    };
    
   
    
    private BeginSimulationEntityCommandBufferSystem m_beginSimECB;
    private EntityQuery m_availableCansQuery;

    protected override void OnCreate()
    {
        m_beginSimECB = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        
        m_availableCansQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] {ComponentType.ReadOnly<Velocity>(),ComponentType.ReadOnly<CanTag>()},
            None = new[] {ComponentType.ReadOnly<CanReservedTag>()}
        });
    }
    

    protected override void OnUpdate()
    {
        var spawnECB = m_beginSimECB.CreateCommandBuffer().ToConcurrent();
        var reserveECB = m_beginSimECB.CreateCommandBuffer();
        float dt = Time.DeltaTime;

        var reserveQueue = new NativeQueue<CanReserveRequest>(Allocator.TempJob);
        //todo query for number of arms in scene and use for capacity parameter
        var reserveMap = new NativeHashMap<Entity, CanReserveRequest>(100, Allocator.TempJob);

        var reserveParallelWriter = reserveQueue.AsParallelWriter();

        var availableCanEntities = m_availableCansQuery.ToEntityArrayAsync(Allocator.TempJob, out var getCansJob);
        
        var canQueueReservesInputDeps = JobHandle.CombineDependencies(Dependency, getCansJob);
        
        var requestJob = Entities
            .WithName("RequestJob")
            .WithAll<ArmGrabbedTag>()
            .WithNone<ArmReservedCan>()
            .ForEach((Entity entity,in ArmAnchorPos anchorPos, in ArmReservedRock reservedRock) =>
        {
            int minIndex = -1;
            float minDistSquared = Single.MaxValue;
            
            for (int i = 0; i < availableCanEntities.Length; i++)
            {
                var can = availableCanEntities[i];
                var canPos = GetComponent<Translation>(can).Value;
                float distSq = math.distancesq(canPos, anchorPos);
                if (distSq < minDistSquared)
                {
                    minDistSquared = distSq;
                    minIndex = i;
                }
            }

            if (minIndex > -1)
            {
                reserveParallelWriter.Enqueue(new CanReserveRequest()
                {
                    armPos = anchorPos,
                    armRef = entity,
                    rockRef = reservedRock, 
                    canRef = availableCanEntities[minIndex]
                });
            }

        }).ScheduleParallel(canQueueReservesInputDeps);

        availableCanEntities.Dispose(requestJob);
        
        
        var  reserveJob = new CanReserveJob
        {
            ecb = reserveECB,
            requestMap = reserveMap,
            requestQueue = reserveQueue,
        }.Schedule(requestJob);

        reserveMap.Dispose(reserveJob);
        reserveQueue.Dispose(reserveJob);
        
        m_beginSimECB.AddJobHandleForProducer(reserveJob);



        Dependency = reserveJob;
    }
}
