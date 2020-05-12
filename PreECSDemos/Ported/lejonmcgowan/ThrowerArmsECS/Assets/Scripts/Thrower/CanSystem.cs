using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(ArmIKSystem))]
public class TinCanSystem: SystemBase
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
                    value = request.canRef
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
            All = new[] {ComponentType.ReadOnly<CanVelocity>()},
            None = new[] {ComponentType.ReadOnly<CanReservedTag>()}
        });
    }
    

    protected override void OnUpdate()
    {
        var destoyECB = m_beginSimECB.CreateCommandBuffer().ToConcurrent();
        var spawnECB = m_beginSimECB.CreateCommandBuffer().ToConcurrent();
        var reserveECB = m_beginSimECB.CreateCommandBuffer();
        float dt = Time.DeltaTime;

        var reserveQueue = new NativeQueue<CanReserveRequest>(Allocator.TempJob);
        //todo query for number of arms in scene and use for capacity parameter
        var reserveMap = new NativeHashMap<Entity, CanReserveRequest>(100, Allocator.TempJob);

        var reserveParallelWriter = reserveQueue.AsParallelWriter();

        var availableCanEntities = m_availableCansQuery.ToEntityArrayAsync(Allocator.TempJob, out var getCansJob);
        
        var velocityJob = Entities
            .WithName("VelocityJob")
            .ForEach((ref Translation position, in CanVelocity velocity) =>
            {
                position.Value += velocity.Value * dt;
            }).ScheduleParallel(Dependency);

        var canQueueReservesInputDeps = JobHandle.CombineDependencies(Dependency, getCansJob, velocityJob);
        
        var requestJob = Entities
            .WithName("RequestJob")
            .WithAll<ArmGrabbedTag>()
            .WithNone<ArmReservedCan>()
            .ForEach((Entity entity,in ArmAnchorPos anchorPos) =>
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
        
        
        var spawnJob = Entities
            .WithName("SpawnJob")
            .ForEach((int entityInQueryIndex, ref CanSpawnComponent spawner) =>
            {
                spawner.spawnTimeRemaining -= dt;
                if (spawner.spawnTimeRemaining <= 0f)
                {
                    spawner.spawnTimeRemaining = spawner.spawnTime;
                    var randPosY = spawner.rng.NextFloat(spawner.yRanges.x, spawner.yRanges.y);
                    var can = spawnECB.Instantiate(entityInQueryIndex,spawner.prefab);
                    spawnECB.AddComponent(entityInQueryIndex,can,new CanVelocity
                    {
                        Value = spawner.spawnVelocity
                        
                    });
                    spawnECB.SetComponent(entityInQueryIndex,can,new Translation
                    {
                        Value = new float3(spawner.xSpawnPos,randPosY,spawner.zSpawnPos)
                    });
                    spawnECB.AddComponent(entityInQueryIndex,can,new CanDestroyBounds
                    {
                        Value = spawner.bounds
                    });
                }
            }).ScheduleParallel(Dependency);
       
        
        //todo: cans should wraparound, never destroy from bounds
        var destroyJob = Entities
            .WithName("WraparoundJob")
            .ForEach((Entity entity,int entityInQueryIndex,
        ref Translation position, in CanDestroyBounds bounds) =>
        {
            if (position.Value.x < bounds.Value.x)
            {
                position.Value.x = bounds.Value.y - 1f;
            }
            else if(position.Value.x > bounds.Value.y)
            {
                position.Value.x = bounds.Value.x + 1f;
            }
        }).ScheduleParallel(reserveJob);
        
        m_beginSimECB.AddJobHandleForProducer(spawnJob);
        m_beginSimECB.AddJobHandleForProducer(destroyJob);

        Dependency = JobHandle.CombineDependencies(reserveJob,spawnJob,destroyJob);
    }
}
