using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
        var stations = CollectionHelper.CreateNativeArray<Entity>(stationConfig.NumStations, Allocator.Temp);

        var em = state.EntityManager;
        em.Instantiate(stationConfig.StationEntity, stations);

        // TODO Make random posiioning work properly
        var random = Random.CreateFromIndex(12314);

        float accumulatedValue = 0;
        foreach (var transform in
            SystemAPI.Query<RefRW<LocalTransform>>()
            .WithAll<StationIDComponent>())
        {
            /*
            var val = random.NextFloat();
            float3 offset = new float3(val * stationConfig.Spacing, 0, 0);

            transform.ValueRW.Position = new float3(i * stationConfig.Spacing, 0, 0) + offset;
            i++;
            */

            var val = random.NextFloat();
            accumulatedValue += (math.max(val * stationConfig.Spacing, 10)) + 30;// + width of the platform
            float3 offset = new float3(accumulatedValue, 0, 0);
            transform.ValueRW.Position = offset;
        }

        for (int trackCount = 0; trackCount < 2; trackCount++)
        {
            var trackEntity = em.CreateEntity();
            em.AddComponent<Track>(trackEntity);
#if UNITY_EDITOR
            em.SetName(trackEntity, "TrackEntity");
#endif
            var TrackPointBuffer = em.AddBuffer<TrackPoint>(trackEntity);

            float3 trackCenter = trackCount % 2 == 0 ? stationConfig.TrackACenter : stationConfig.TrackBCenter;
            int i = 0;
            foreach (var transform in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<StationIDComponent>())
            {
                bool isEnd = i == 0 || i == stationConfig.NumStations - 1;
                TrackPointBuffer.Add(new TrackPoint { Position = trackCenter + transform.ValueRO.Position + new float3(-20, 0, 0) });
                TrackPointBuffer.Add(new TrackPoint { Position = trackCenter + transform.ValueRO.Position, IsEnd = isEnd, IsStation = true });
                TrackPointBuffer.Add(new TrackPoint { Position = trackCenter + transform.ValueRO.Position + new float3(+20, 0, 0) });
                i++;
            }

            float sleeperSpacing = 1;
            for (int j = 0; j < TrackPointBuffer.Length - 1; j++)
            {
                float distance = math.distance(TrackPointBuffer[j].Position, TrackPointBuffer[j + 1].Position);
                var numberOfSleepers = (int)(distance / sleeperSpacing);

                var sleepers = CollectionHelper.CreateNativeArray<Entity>(numberOfSleepers, Allocator.Temp);
                em.Instantiate(stationConfig.TrackEntity, sleepers);

                for (int k = 0; k < sleepers.Length; k++)
                {
                    float3 pos = math.lerp(TrackPointBuffer[j].Position, TrackPointBuffer[j + 1].Position, (float)k / sleepers.Length);
                    LocalTransform lc = new LocalTransform();
                    lc.Position = pos;
                    lc.Scale = 1;
                    lc.Rotation = quaternion.RotateY(math.PI / 2);
                    em.SetComponentData<LocalTransform>(sleepers[k], lc);
                }
            }
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

        state.Enabled = false;
    }
}
