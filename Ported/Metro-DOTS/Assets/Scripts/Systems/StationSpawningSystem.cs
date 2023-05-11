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
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        var stationConfig = SystemAPI.GetSingleton<StationConfig>();
        var totalNumStations = stationConfig.NumStations * stationConfig.NumLines;
        var stations = CollectionHelper.CreateNativeArray<Entity>(totalNumStations, Allocator.Temp);

        var em = state.EntityManager;
        em.Instantiate(stationConfig.StationEntity, stations);

        // TODO Make random positioning work properly
//         var random = Random.CreateFromIndex(12314);
//
//         int i = 0;
//         float accumulatedValue = 0;
//         foreach (var transform in
//             SystemAPI.Query<RefRW<LocalTransform>>()
//             .WithAll<StationIDComponent>())
//         {
//             /*
//             var val = random.NextFloat();
//             float3 offset = new float3(val * stationConfig.Spacing, 0, 0);
//
//             transform.ValueRW.Position = new float3(i * stationConfig.Spacing, 0, 0) + offset;
//             i++;
//             */
//
//             var val = random.NextFloat();
//             accumulatedValue += (math.max(val * stationConfig.Spacing, 10)) + stationConfig.StationWidth;// + width of the platform
//             float3 offset = new float3(accumulatedValue, 0, 0);
//             transform.ValueRW.Position = offset;
//         }
        
        var trackArchetype = em.CreateArchetype(typeof(TrackIDComponent));
        var numTracks = stationConfig.NumLines * 2;
        var trackEntities = CollectionHelper.CreateNativeArray<Entity>(numTracks, Allocator.Temp);
        em.CreateEntity(trackArchetype, trackEntities);
        
        // var trackEntityA = em.CreateEntity(trackArchetype);
        // var trackEntityB = em.CreateEntity(trackArchetype);
        for (var t = 0; t < trackEntities.Length; t++)
        {
#if UNITY_EDITOR
            em.SetName(trackEntities[t], $"TrackEntity{(t % 2 == 0 ? "A" : "B")}");
#endif
            em.AddBuffer<TrackPoint>(trackEntities[t]);

            // em.SetName(trackEntityA, "TrackEntityA");
            // em.SetName(trackEntityB, "TrackEntityB");
            //var TrackPointBufferA = em.AddBuffer<TrackPoint>(trackEntityA);
            //var TrackPointBufferB = em.AddBuffer<TrackPoint>(trackEntityB);
            // TrackPointBufferA = em.GetBuffer<TrackPoint>(trackEntityA);
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
            bufferA.Add(new TrackPoint { Position = trackAOffset + stationOffset - trackPointOffsetFromCenter, Station = Entity.Null });
            bufferA.Add(new TrackPoint { Position = trackAOffset + stationOffset, IsEnd = isEnd, IsStation = true, Station = stationEntity});
            bufferA.Add(new TrackPoint { Position = trackAOffset + stationOffset + trackPointOffsetFromCenter, Station = Entity.Null  });
            
            bufferB.Add(new TrackPoint { Position = trackBOffset + stationOffset - trackPointOffsetFromCenter, Station = Entity.Null  });
            bufferB.Add(new TrackPoint { Position = trackBOffset + stationOffset, IsEnd = isEnd, IsStation = true, Station = stationEntity });
            bufferB.Add(new TrackPoint { Position = trackBOffset + stationOffset + trackPointOffsetFromCenter, Station = Entity.Null  });
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
            
        //}
        
        
        //foreach (var trackBuffer in new[] { TrackPointBufferA, TrackPointBufferB })
        //{
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

        // TODO Add random spawn points between 3-5 for same number of carriadges
        // var random = Random.CreateFromIndex(12314);
        // var val = random.NextFloat();
        int numQueuePoints = stationConfig.NumStations * stationConfig.NumLines * (stationConfig.NumQueingPoints /** 2*/);
        var queuePoints = CollectionHelper.CreateNativeArray<Entity>(numQueuePoints, Allocator.Temp);

        EntityArchetype queueArchetype = em.CreateArchetype(typeof(QueueComponent), typeof(LocalTransform), typeof(QueuePassengers));
#if UNITY_EDITOR
        // todo: make naming work
        for (var k = 0; k < queuePoints.Length; k++)
        {
            em.SetName(queuePoints[k], "QueueEntity");   
        }
#endif
        em.CreateEntity(queueArchetype, queuePoints);

		for (int j = 0; j < queuePoints.Length; j++)
        {
            // Todo is there a better way to initialize 16 elemtns into a buffer?
            var buffer = em.GetBuffer<QueuePassengers>(queuePoints[j]);
            for (int k = 0; k < 16; k++)
            {
                buffer.Add(new QueuePassengers());
            }

            // init QueueComponent
            em.SetComponentData<QueueComponent>(queuePoints[j], new QueueComponent
            {
                StartIndex = -1,
                QueueLength = 0
            });
        }
        float carriageLength = 5;

        i = 0;
        foreach (var transform in
            SystemAPI.Query<RefRO<LocalTransform>>()
            .WithAll<StationIDComponent>())
        {
			var stationsQueueBuffer = em.GetBuffer<StationQueuesElement>(stations[i]);
            for (int k = 0; k < stationConfig.NumQueingPoints; k++)
            {
                LocalTransform lc = new LocalTransform();
                float totalQueuePointsSpan = (carriageLength * (stationConfig.NumQueingPoints - 1)) / 2;
                lc.Position = stationConfig.TrackACenter + stationConfig.SpawnPointOffsetFromCenterPoint + transform.ValueRO.Position - new float3(totalQueuePointsSpan, 0, 0) + new float3(k * carriageLength, 0, 0);
                lc.Scale = 1;
				lc.Rotation = quaternion.RotateY(math.PI);
                var queuePointIndex = (i * stationConfig.NumQueingPoints) + k;
                em.SetComponentData<LocalTransform>(queuePoints[queuePointIndex], lc);
				
				stationsQueueBuffer.Add(new StationQueuesElement { Queue = queuePoints[queuePointIndex] });
            }

            i++;
            //for (int k = 0; k < stationConfig.NumCarriadges; k++)
            //{
            //    // make the x negative
            //}
        }

        // var tracks = CollectionHelper.CreateNativeArray<Entity>(TrackPointBuffer.Length, Allocator.Temp);
        // em.Instantiate(stationConfig.TrackEntity, tracks);

        // i = 0;
        // foreach (var transform in
        //     SystemAPI.Query<RefRW<LocalTransform>>()
        //     .WithAll<SleeperTag>())
        // {
        //     transform.ValueRW.Position = TrackPointBuffer[i].Position;
        //     transform.ValueRW.Rotation = quaternion.RotateY(math.PI / 2);
        //     i++;
        // }


        /*
        var query = SystemAPI.QueryBuilder().WithAll<StationConfig>().Build();
        var chunks = query.ToArchetypeChunkArray(Allocator.Temp);
        var localTransformHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>();
        foreach (var chunk in chunks)
        {
            var localTransformVals = chunk.GetNativeArray(localTransformHandle);
            for (int j = 0; j < chunk.Count; j++)
            {
                var v = localTransformVals[j];
            }
        }
        */
        
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
