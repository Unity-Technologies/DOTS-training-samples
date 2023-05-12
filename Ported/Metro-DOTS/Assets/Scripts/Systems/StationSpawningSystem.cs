using Components;
using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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
        var totalNumStations = stationConfig.NumStations * stationConfig.NumLines;
        var stations = CollectionHelper.CreateNativeArray<Entity>(totalNumStations, Allocator.Temp);

        var em = state.EntityManager;
        em.Instantiate(stationConfig.StationEntity, stations);

        var trackArchetype = em.CreateArchetype(typeof(TrackIDComponent), typeof(Track));
        var numTracks = stationConfig.NumLines * 2;
        var trackEntities = CollectionHelper.CreateNativeArray<Entity>(numTracks, Allocator.Temp);
        em.CreateEntity(trackArchetype, trackEntities);
        
        for (int t = 0; t < trackEntities.Length; t++)
        {
			bool isTrackA = t % 2 == 0;
			var trackEntity = trackEntities[t];
#if UNITY_EDITOR
            em.SetName(trackEntity, $"TrackEntity{(isTrackA ? "A" : "B")}");
#endif
            em.AddBuffer<TrackPoint>(trackEntity);
			em.SetComponentData<Track>(trackEntity, new Track
            {
                OnPlatformA = isTrackA,
                LineID = t / 2
            });
        }

        var random = Random.CreateFromIndex(12314);
        int i = 0;
        int lineIndex = 0;
		int stationIdCount = 0;
        float accumulatedValue = 0;
        var halfStationLength = 20;
        float3 trackPointOffsetFromCenter = new float3(halfStationLength, 0, 0);
        foreach (var (transform, stationID, stationEntity) in
            SystemAPI.Query<RefRW<LocalTransform>, RefRW<StationIDComponent>>()
            .WithEntityAccess())
        {
			stationID.ValueRW.StationID = stationIdCount;
            stationID.ValueRW.LineID = lineIndex;

            stationIdCount++;
		
            var randomVal = random.NextFloat();
            var trackEntityA = trackEntities[lineIndex * 2];
            var trackEntityB = trackEntities[lineIndex * 2 + 1];
            var bufferA = em.GetBuffer<TrackPoint>(trackEntityA);
            var bufferB = em.GetBuffer<TrackPoint>(trackEntityB);

            var lineOffset = new float3(0, 0, lineIndex * stationConfig.LineSpacing);
            var trackAOffset = stationConfig.TrackACenter + lineOffset;
            var trackBOffset = stationConfig.TrackBCenter + lineOffset;
            
            accumulatedValue += (math.max(randomVal * stationConfig.Spacing, 10)) + stationConfig.StationWidth;// + width of the platform
            float3 stationOffset = new float3(accumulatedValue, 0, 0);
            transform.ValueRW.Position = stationOffset + lineOffset;
            
            bool isEnd = i == 0 || i == stationConfig.NumStations - 1;
            bufferA.Add(new TrackPoint { Position = trackAOffset + stationOffset - trackPointOffsetFromCenter});
            bufferA.Add(new TrackPoint { Position = trackAOffset + stationOffset, IsEnd = isEnd, IsStation = true, Station = stationEntity});
            bufferA.Add(new TrackPoint { Position = trackAOffset + stationOffset + trackPointOffsetFromCenter});
            
            bufferB.Add(new TrackPoint { Position = trackBOffset + stationOffset - trackPointOffsetFromCenter});
            bufferB.Add(new TrackPoint { Position = trackBOffset + stationOffset, IsEnd = isEnd, IsStation = true, Station = stationEntity });
            bufferB.Add(new TrackPoint { Position = trackBOffset + stationOffset + trackPointOffsetFromCenter});
            i++;
            if (i == stationConfig.NumStations)
            {
                i = 0;
                accumulatedValue = 0;
                lineIndex++;
            }
        }

        float sleeperSpacing = 1;

        for (int trackIndex = 0; trackIndex < trackEntities.Length; trackIndex++)
        {
            var trackBuffer = em.GetBuffer<TrackPoint>(trackEntities[trackIndex]);
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
        int numQueuePoints = stationConfig.NumStations * (stationConfig.NumQueingPointsPerPlatform * stationConfig.NumLines * 2);
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

        i = 0;
        foreach (var (transform, stationInfo, station) in
            SystemAPI.Query<RefRO<LocalTransform>, RefRO<StationIDComponent>>()
                .WithEntityAccess()
                .WithAll<StationIDComponent>())
        {
            var stationsQueueBuffer = em.GetBuffer<StationQueuesElement>(stations[i]);

            for (int k = 0; k < stationConfig.NumQueingPointsPerPlatform; k++)
            {
                // queues on platform A
                float totalQueuePointsSpan = (config.CarriageLength * (stationConfig.NumQueingPointsPerPlatform - 1f)) / 2f;
                LocalTransform lcA = new LocalTransform
                {
                    Position = stationConfig.TrackACenter + stationConfig.SpawnPointOffsetFromCenterPoint +
                               transform.ValueRO.Position - new float3(totalQueuePointsSpan, 0, 0) +
                               new float3(k * config.CarriageLength, 0, 0),
                    Scale = 1f,
                    Rotation = quaternion.RotateY(math.PI)
                };
                var queuePointAIndex = i * stationConfig.NumQueingPointsPerPlatform * 2 + k * 2;
                em.SetComponentData<LocalTransform>(queuePoints[queuePointAIndex], lcA);

                var queueComponent = em.GetComponentData<QueueComponent>(queuePoints[queuePointAIndex]);
                queueComponent.Station = station;
                queueComponent.OnPlatformA = true;
                queueComponent.LineID = stationInfo.ValueRO.LineID;
                em.SetComponentData(queuePoints[queuePointAIndex], queueComponent);

                stationsQueueBuffer.Add(new StationQueuesElement { Queue = queuePoints[queuePointAIndex] });
                
                // queues on platform B
                LocalTransform lcB = new LocalTransform
                {
                    Position = stationConfig.TrackBCenter + stationConfig.SpawnPointOffsetFromCenterPoint * new float3(1f, 1f, -1f) +
                               transform.ValueRO.Position - new float3(totalQueuePointsSpan, 0, 0) +
                               new float3(k * config.CarriageLength, 0, 0),
                    Scale = 1f
                };
                var queuePointBIndex = i * stationConfig.NumQueingPointsPerPlatform * 2 + k * 2 + 1;
                em.SetComponentData<LocalTransform>(queuePoints[queuePointBIndex], lcB);

                var queueBInfo = em.GetComponentData<QueueComponent>(queuePoints[queuePointAIndex]);
                queueBInfo.Station = station;
                queueBInfo.OnPlatformA = false;
                queueBInfo.LineID = stationInfo.ValueRO.LineID;
                em.SetComponentData(queuePoints[queuePointBIndex], queueBInfo);

                stationsQueueBuffer.Add(new StationQueuesElement { Queue = queuePoints[queuePointBIndex] });
            }
            i++;
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var query = SystemAPI.QueryBuilder().WithAll<URPMaterialPropertyBaseColor>().Build();
        var queryMask = query.GetEntityQueryMask();
        
        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        var hue = random.NextFloat();

        // Set the color of the station
        // Helper to create any amount of colors as distinct from each other as possible.
        // The logic behind this approach is detailed at the following address:
        // https://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
        URPMaterialPropertyBaseColor RandomColor()
        {
            // Note: if you are not familiar with this concept, this is a "local function".
            // You can search for that term on the internet for more information.

            // 0.618034005f == 2 / (math.sqrt(5) + 1) == inverse of the golden ratio
            hue = (hue + 0.618034005f) % 1;
            var color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (Vector4)color };
        }

        var stationColor = RandomColor();

        foreach (var station in stations)
        {
            ecb.SetComponentForLinkedEntityGroup(station, queryMask, stationColor);
        }

        state.Enabled = false;
    }
}
