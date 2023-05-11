using Components;
using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct StationSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<StationConfig>();
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        var stationConfig = SystemAPI.GetSingleton<StationConfig>();
        var config = SystemAPI.GetSingleton<Config>();
        var stations = CollectionHelper.CreateNativeArray<Entity>(stationConfig.NumStations, Allocator.Temp);

        var em = state.EntityManager;
        em.Instantiate(stationConfig.StationEntity, stations);

        // TODO Make random posiioning work properly
        var random = Random.CreateFromIndex(12314);

        int stationIdCount = 0;
        float accumulatedValue = 0;
        foreach (var (transform, stationID) in
            SystemAPI.Query<RefRW<LocalTransform>, RefRW<StationIDComponent>>())
        {
            /*
            var val = random.NextFloat();
            float3 offset = new float3(val * stationConfig.Spacing, 0, 0);

            transform.ValueRW.Position = new float3(i * stationConfig.Spacing, 0, 0) + offset;
            i++;
            */

            stationID.ValueRW.StationID = stationIdCount;
            stationIdCount++;

            var val = random.NextFloat();
            accumulatedValue += (math.max(val * stationConfig.Spacing, 10)) + stationConfig.StationWidth;// + width of the platform
            float3 offset = new float3(accumulatedValue, 0, 0);
            transform.ValueRW.Position = offset;
        }

        var trackArchetype = em.CreateArchetype(typeof(TrackIDComponent), typeof(Track));
        var trackEntityA = em.CreateEntity(trackArchetype);
        var trackEntityB = em.CreateEntity(trackArchetype);
#if UNITY_EDITOR
        em.SetName(trackEntityA, "TrackEntityA");
        em.SetName(trackEntityB, "TrackEntityB");
#endif
        var TrackPointBufferA = em.AddBuffer<TrackPoint>(trackEntityA);
        var TrackPointBufferB = em.AddBuffer<TrackPoint>(trackEntityB);
        TrackPointBufferA = em.GetBuffer<TrackPoint>(trackEntityA);

        int i = 0;
        var halfStationLength = 20;
        float3 trackPointOffsetFromCenter = new float3(halfStationLength, 0, 0);
        foreach (var (transform, stationEntity) in
            SystemAPI.Query<RefRO<LocalTransform>>()
            .WithAll<StationIDComponent>()
            .WithEntityAccess())
        {
            bool isEnd = i == 0 || i == stationConfig.NumStations - 1;
            TrackPointBufferA.Add(new TrackPoint { Position = stationConfig.TrackACenter + transform.ValueRO.Position - trackPointOffsetFromCenter, Station = Entity.Null });
            TrackPointBufferA.Add(new TrackPoint { Position = stationConfig.TrackACenter + transform.ValueRO.Position, IsEnd = isEnd, IsStation = true, Station = stationEntity});
            TrackPointBufferA.Add(new TrackPoint { Position = stationConfig.TrackACenter + transform.ValueRO.Position + trackPointOffsetFromCenter, Station = Entity.Null  });
            
            TrackPointBufferB.Add(new TrackPoint { Position = stationConfig.TrackBCenter + transform.ValueRO.Position - trackPointOffsetFromCenter, Station = Entity.Null  });
            TrackPointBufferB.Add(new TrackPoint { Position = stationConfig.TrackBCenter + transform.ValueRO.Position, IsEnd = isEnd, IsStation = true, Station = stationEntity });
            TrackPointBufferB.Add(new TrackPoint { Position = stationConfig.TrackBCenter + transform.ValueRO.Position + trackPointOffsetFromCenter, Station = Entity.Null  });
            i++;
        }

        float sleeperSpacing = 1;

        foreach (var trackBuffer in new[] { TrackPointBufferA, TrackPointBufferB })
        {
            for (int j = 0; j < trackBuffer.Length - 1; j++)
            {
                float distance = math.distance(trackBuffer[j].Position, trackBuffer[j + 1].Position);
                var numberOfSleepers = (int)(distance / sleeperSpacing);

                var sleepers = CollectionHelper.CreateNativeArray<Entity>(numberOfSleepers, Allocator.Temp);
                em.Instantiate(stationConfig.TrackEntity, sleepers);

                for (int k = 0; k < sleepers.Length; k++)
                {
                    float3 pos = math.lerp(trackBuffer[j].Position, trackBuffer[j + 1].Position, (float)k / sleepers.Length);
                    LocalTransform lc = new LocalTransform
                    {
                        Position = pos,
                        Scale = 1f,
                        Rotation = quaternion.RotateY(math.PI / 2)
                    };
                    em.SetComponentData<LocalTransform>(sleepers[k], lc);
                }
            }
        }

        // TODO Add random spawn points between 3-5 for same number of carriages
        int numQueuePoints = stationConfig.NumStations * (stationConfig.NumQueingPointsPerPlatform * 2);
        var queuePoints = CollectionHelper.CreateNativeArray<Entity>(numQueuePoints, Allocator.Temp);
        em.Instantiate(stationConfig.QueueEntity, queuePoints);

        for (int j = 0; j < queuePoints.Length; j++)
        {
            var buffer = em.GetBuffer<QueuePassengers>(queuePoints[j]);
            buffer.ResizeUninitialized(config.MaxPassengerPerQueue);

            // init QueueComponent
            em.SetComponentData<QueueComponent>(queuePoints[j], new QueueComponent
            {
                StartIndex = -1,
                QueueLength = 0
            });
        }

        float carriadgeLength = 5;

        i = 0;
        foreach (var (transform, station) in
            SystemAPI.Query<RefRO<LocalTransform>>()
                .WithEntityAccess()
                .WithAll<StationIDComponent>())
        {
            var stationsQueueBuffer = em.GetBuffer<StationQueuesElement>(stations[i]);

            for (int k = 0; k < stationConfig.NumQueingPointsPerPlatform; k++)
            {
                float totalQueuePointsSpan = (carriadgeLength * (stationConfig.NumQueingPointsPerPlatform - 1f)) / 2f;
                LocalTransform lc = new LocalTransform
                {
                    Position = stationConfig.TrackACenter + stationConfig.SpawnPointOffsetFromCenterPoint +
                               transform.ValueRO.Position - new float3(totalQueuePointsSpan, 0, 0) +
                               new float3(k * carriadgeLength, 0, 0),
                    Scale = 1f,
                    Rotation = quaternion.RotateY(math.PI)
                };
                var queuePointIndex = i * stationConfig.NumQueingPointsPerPlatform * 2 + k * 2;
                em.SetComponentData<LocalTransform>(queuePoints[queuePointIndex], lc);

                var queueComponent = em.GetComponentData<QueueComponent>(queuePoints[queuePointIndex]);
                queueComponent.Station = station;
                em.SetComponentData(queuePoints[queuePointIndex], queueComponent);

                stationsQueueBuffer.Add(new StationQueuesElement { Queue = queuePoints[queuePointIndex] });
                
                LocalTransform lcB = new LocalTransform
                {
                    Position = stationConfig.TrackBCenter + stationConfig.SpawnPointOffsetFromCenterPoint * new float3(1f, 1f, -1f) +
                               transform.ValueRO.Position - new float3(totalQueuePointsSpan, 0, 0) +
                               new float3(k * carriadgeLength, 0, 0),
                    Scale = 1f
                };
                var queuePointBIndex = i * stationConfig.NumQueingPointsPerPlatform * 2 + k * 2 + 1;
                em.SetComponentData<LocalTransform>(queuePoints[queuePointBIndex], lcB);

                var queueBInfo = em.GetComponentData<QueueComponent>(queuePoints[queuePointIndex]);
                queueBInfo.Station = station;
                em.SetComponentData(queuePoints[queuePointBIndex], queueBInfo);

                stationsQueueBuffer.Add(new StationQueuesElement { Queue = queuePoints[queuePointBIndex] });
            }
            i++;
        }

        state.Enabled = false;
    }
}
