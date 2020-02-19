using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;

public enum CommuterState : int
{
    WALK,
    QUEUE,
    GET_ON_TRAIN,
    GET_OFF_TRAIN,
    WAIT_FOR_STOP,
}

public struct CommuterSpawn : IComponentData
{
    public Entity Prefab;
    public int Count;
    public uint Seed;
}

public struct RoutingData : IComponentData
{
    public float3 Destination;
    public CommuterState State;
    public Entity CurPlatform;
    public float StateDelay;
    public int TrainCarIndex;
    public int QueueIndex;
    public uint RandomState;
}

public struct TrainCarIndex : IComponentData
{
    public int Value;
}

public struct ColorData : IComponentData
{
    public float3 Value;
}

public struct QueueId : IEquatable<QueueId>
{
    public Entity Platform;
    public int QueueIndex;

    public bool Equals(QueueId other)
    {
        return Platform.Equals(other.Platform) && QueueIndex == other.QueueIndex;
    }

    public override bool Equals(object obj)
    {
        return obj is QueueId other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Platform.GetHashCode() * 397) ^ QueueIndex;
        }
    }
}
public struct QueueRequest : IComponentData
{
    public Entity Commuter;
    public QueueId QueueId;
}

public class CommuterSpawnSystem : JobComponentSystem
{
    public const float ACCELERATION_STRENGTH = 0.01f;

    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();
        EntityQuery platformQuery = GetEntityQuery(
            ComponentType.ReadOnly<PlatformNavPoints>(), 
            ComponentType.ReadOnly<QueuePosition>());

        var platforms = platformQuery.ToEntityArray(Allocator.TempJob);
        var numPlatforms = platforms.Length;
        var platformTxLookup = GetComponentDataFromEntity<LocalToWorld>(true);
        var navPointsLookup = GetComponentDataFromEntity<PlatformNavPoints>(true);
        var queuePointsLookup = GetBufferFromEntity<QueuePosition>(true);

        var commuterSpawnJob = Entities
            .WithReadOnly(navPointsLookup)
            .WithReadOnly(queuePointsLookup)
            .WithReadOnly(platformTxLookup)
            .WithDeallocateOnJobCompletion(platforms)
            .ForEach((Entity entity, CommuterSpawn spawn, int entityInQueryIndex) =>
        {
            var rand = new Unity.Mathematics.Random(spawn.Seed);
            var count = spawn.Count;
            for (var i = 0; i < count; i++)
            {
                var instance = ecb.Instantiate(entityInQueryIndex, spawn.Prefab);

                var platformIdx = rand.NextInt(numPlatforms);
                var platform = platforms[platformIdx];
                var platformTx = platformTxLookup[platform].Value;
                var navPoints = navPointsLookup[platform];
                var queuePoints = queuePointsLookup[platform];

                var side = rand.NextBool();
                var position = side ? navPoints.backUp : navPoints.frontUp;
                position = math.mul(platformTx, float4(position, 1f)).xyz;
                var dest = side ? navPoints.backDown : navPoints.frontDown;
                dest = math.mul(platformTx, float4(dest, 1f)).xyz;

                ecb.SetComponent(entityInQueryIndex, instance, new Translation {Value = position});
                
                // random size
                ecb.AddComponent(entityInQueryIndex, instance, new NonUniformScale
                {
                    Value = float3(1.0f, rand.NextFloat(0.25f, 1.5f), 1.0f)
                });
                
                ecb.AddComponent(entityInQueryIndex, instance, new MovementDerivatives
                {
                    Acceleration = ACCELERATION_STRENGTH * rand.NextFloat(0.8f, 2f),
                });

                // random speed
                ecb.AddComponent(entityInQueryIndex, instance, new RoutingData
                {
                    State = CommuterState.WALK,
                    Destination = dest,
                    CurPlatform = platform,
                    RandomState = rand.NextUInt() // FIXME: this is hilariously bad
                });
                
                // random Color
                ecb.AddComponent(entityInQueryIndex, instance, new ColorData
                {
                    Value = float3(rand.NextFloat(0f, 1f), rand.NextFloat(0f, 1f), rand.NextFloat(0f, 1f))
                });
            }
            
            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).Schedule(inputDependencies);
        
        ecbSystem.AddJobHandleForProducer(commuterSpawnJob);
        
        return commuterSpawnJob;
    }
}

[UpdateAfter(typeof(TrainPositioningSystem))]
public class CommuterSystem : JobComponentSystem
{
    public const float FRICTION = 0.8f;
    public const float ARRIVAL_THRESHOLD = 0.02f;
    public const float QUEUE_PERSONAL_SPACE = 0.4f;
    public const float QUEUE_MOVEMENT_DELAY = 0.25f;
    public const float QUEUE_DECISION_RATE = 3f;

    private NativeMultiHashMap<QueueId, Entity> QueueMap;

    private EntityCommandBufferSystem BeginSim;
    private EntityCommandBufferSystem EndSim;

    protected override void OnCreate()
    {
        QueueMap = new NativeMultiHashMap<QueueId, Entity>(0, Allocator.Persistent);
        BeginSim = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        EndSim = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        QueueMap.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var processQueueRequests = new QueueRequestJob
        {
            QueueMap = QueueMap,
            CommuterLookup = GetComponentDataFromEntity<RoutingData>(),
            Ecb = BeginSim.CreateCommandBuffer()
        }.ScheduleSingle(this, inputDependencies);
        BeginSim.AddJobHandleForProducer(processQueueRequests);
        
        var dt = Time.DeltaTime;
        var platformTxLookup = GetComponentDataFromEntity<LocalToWorld>(true);
        var queuePointsLookup = GetBufferFromEntity<QueuePosition>(true);
        var queueMap = QueueMap;
        var ecb = EndSim.CreateCommandBuffer().ToConcurrent();
        var commuterUpdateJob = Entities
            .WithReadOnly(platformTxLookup)
            .WithReadOnly(queuePointsLookup)
            .WithReadOnly(queueMap)
            .ForEach((Entity entity, int entityInQueryIndex, 
                ref RoutingData commuter, 
                ref MovementDerivatives movement,
                ref Translation translation) =>
        {
            if (commuter.State == CommuterState.WALK)
            {
                if (Approach.Apply(ref translation.Value, ref movement.Speed, commuter.Destination,
                    movement.Acceleration, ARRIVAL_THRESHOLD, FRICTION))
                {
                    commuter.State = CommuterState.QUEUE;
                    
                    commuter.StateDelay = QUEUE_DECISION_RATE;

                    var rand = new Random();
                    rand.InitState(commuter.RandomState);
                    var queuePoints = queuePointsLookup[commuter.CurPlatform];
                    var queueIndex = ShortestQueue(queuePoints, ref rand);
                    commuter.TrainCarIndex = queueIndex; // TODO: AddComponentData(entity, new TrainCarIndex{Value = carriageQueueIndex});
                    commuter.RandomState = rand.state;
                }
            }
            else if (commuter.State == CommuterState.QUEUE)
            {
                var queueId = new QueueId
                {
                    Platform = commuter.CurPlatform,
                    QueueIndex = commuter.TrainCarIndex
                };
                var indexInQueue = queueMap.CountValuesForKey(queueId);
                float offset = indexInQueue * QUEUE_PERSONAL_SPACE;
                var platformTx = platformTxLookup[commuter.CurPlatform].Value;
                float3 forward = math.forward(quaternion(platformTx));
                float3 queueOffset = forward * offset;
                var queuePoints = queuePointsLookup[commuter.CurPlatform];
                float3 queuePoint = queuePoints[commuter.TrainCarIndex];
                commuter.Destination = math.mul(platformTx, float4(queuePoint, 1f)).xyz + queueOffset;
                commuter.QueueIndex = indexInQueue;

                if (Approach.Apply(ref translation.Value, ref movement.Speed, commuter.Destination,
                    movement.Acceleration, ARRIVAL_THRESHOLD, FRICTION))
                {
                    ecb.AddComponent(entityInQueryIndex, ecb.CreateEntity(entityInQueryIndex), new QueueRequest
                    {
                        Commuter = entity,
                        QueueId = queueId
                    });
                    // TODO: wait for train to open
                    //commuter.State = CommuterState.GET_ON_TRAIN;
                }
            }
        }).Schedule(processQueueRequests);
        
        EndSim.AddJobHandleForProducer(commuterUpdateJob);
        
        return commuterUpdateJob;
    }

    struct QueueRequestJob : IJobForEachWithEntity<QueueRequest>
    {
        public NativeMultiHashMap<QueueId, Entity> QueueMap;
        public ComponentDataFromEntity<RoutingData> CommuterLookup;
        public EntityCommandBuffer Ecb;
        public void Execute(Entity entity, int index, ref QueueRequest queueRequest)
        {
            var commuterEntity = queueRequest.Commuter;
            var commuter = CommuterLookup[commuterEntity];
         
            var queueId = queueRequest.QueueId;
            var count = QueueMap.CountValuesForKey(queueId);

            QueueMap.Add(queueId, commuterEntity);
            commuter.QueueIndex = count;
            commuter.State = CommuterState.GET_ON_TRAIN;
            Ecb.SetComponent(commuterEntity, commuter);
            Ecb.DestroyEntity(entity);
        }
    }

    private static int ShortestQueue(DynamicBuffer<QueuePosition> queuePoints, ref Random rand)
    {
        var count = queuePoints.Length;
        return rand.NextInt(count);
    }
}