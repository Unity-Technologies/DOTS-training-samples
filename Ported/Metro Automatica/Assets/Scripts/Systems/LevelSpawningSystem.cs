using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
partial struct LevelSpawningSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var config = SystemAPI.GetSingleton<Config>();

        var stations = CollectionHelper.CreateNativeArray<Entity>(config.StationCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.StationPrefab, stations);
        
        var wagons = CollectionHelper.CreateNativeArray<Entity>(config.WagonCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.WagonPrefab, wagons);

        LocalTransform first = new LocalTransform();
        LocalTransform last = new LocalTransform();

        for (int i = 0; i < stations.Length; i++)
        {
            var transform = LocalTransform.FromPosition(i * 90, 0, 0);
            state.EntityManager.SetComponentData(stations[i], transform);

            var queuePoints = state.EntityManager.GetBuffer<QueueWaypointCollection>(stations[i]);
            var bridgePoints = state.EntityManager.GetBuffer<BridgeWaypointCollection>(stations[i]);
            for (int j = 0; j < queuePoints.Length; j++)
            {
                var offSetLocation = queuePoints[j];
                offSetLocation.North.x += i * 90;
                offSetLocation.South.x += i * 90;
                queuePoints[j] = offSetLocation;
            }
            for (int j = 0; j < bridgePoints.Length; j++)
            {
                var offSetLocation = bridgePoints[j];
                offSetLocation.West.x += i * 90;
                offSetLocation.East.x += i * 90;
                bridgePoints[j] = offSetLocation;
            }
            
            var spawnerTransform = WorldTransform.FromPosition(transform.Position + state.EntityManager.GetComponentData<Station>(stations[i]).HumanSpawnerLocation.Position);
            Station tempStation = new Station() { HumanSpawnerLocation = spawnerTransform };
            state.EntityManager.SetComponentData(stations[i], tempStation);
            
            if (i == 0)
            {
                first = transform;
                var firstWagon = transform;
                for (int j = 0; j < wagons.Length ; j++)
                {
                    var wagonTransform = LocalTransform.FromPosition(firstWagon.Position.x, 0, 0);
                    wagonTransform.Rotation = quaternion.RotateY(math.radians(90));
                    state.EntityManager.SetComponentData(wagons[j], wagonTransform);
                    var tempWagon = SystemAPI.GetComponent<Wagon>(wagons[j]);
                    tempWagon.TrainOffset = firstWagon.Position.x;
                    tempWagon.StationCounter = i;
                    tempWagon.Direction = 1;
                    SystemAPI.SetComponent(wagons[j], tempWagon);
                    firstWagon.Position.x += 5.2f;
                }
            }

            for (int j = 0; j < wagons.Length; j++)
            {
                var wagonStationBuffer = SystemAPI.GetBuffer<StationWayPoints>(wagons[j]);
                float3 stationLocation = new float3(i * 90f, 0, 0);
                wagonStationBuffer.Add(new StationWayPoints{Value = stationLocation});
            }

            if (i == stations.Length -1)
            {
                last = transform;
            }
            
                    
            // foreach (var stationTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Station>())
            // {
            //     var wagonStationBuffer = SystemAPI.GetBuffer<StationWayPoints>(wagons[j]);
            //     wagonStationBuffer.Add(new StationWayPoints{Value = stationTransform.ValueRO.Position});
            // }
            //Debug.Log($"First: {first.Position.x}");
        }

        

        // This system should only run once at startup. So it disables itself after one update.
        var railSpawn = state.EntityManager.Instantiate(config.RailPrefab);
        
        // need var distance /2 between first and last station in x value (for now)

        var railTransform = LocalTransform.FromPosition(first.Position.x + (last.Position.x - first.Position.x)/2, -1, 0);
        
        // need var distance to calculate scale value

        float stretch = last.Position.x - first.Position.x;

        state.EntityManager.SetComponentData(railSpawn, new PostTransformScale{Value = float3x3.Scale(stretch,.1f,1)});
        state.EntityManager.SetComponentData(railSpawn, new RailStretchFactor{StretchFactor = new float2(stretch, 1)});

        state.EntityManager.SetComponentData(railSpawn, railTransform);
        
        
        var humans = CollectionHelper.CreateNativeArray<Entity>(config.HumanCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.HumanPrefab, humans);

        NativeList<WorldTransform> stationSpawners = new NativeList<WorldTransform>(Allocator.Temp);
        
        foreach (var station in SystemAPI.Query<RefRW<Station>>())
        {
           // Debug.Log("I have found some stations");
            stationSpawners.Add(station.ValueRO.HumanSpawnerLocation);
        }

        Unity.Mathematics.Random rng = new Unity.Mathematics.Random(123);
        foreach (var human in humans)
        {
            int randomStation = Random.Range(0, config.StationCount);
            var humanTransform = LocalTransform.FromPosition(stationSpawners[randomStation].Position);
            var height = rng.NextFloat(.4f, 0.6f);
            humanTransform.Position += new float3(Random.Range(0f, 2f), (1 - height)/-2, Random.Range(0f, 2f));
            state.EntityManager.SetComponentData(human, humanTransform);
            state.EntityManager.SetComponentData(human, new PostTransformScale{Value = float3x3.Scale(.25f,height,.25f)});
            state.EntityManager.SetComponentData(human, new URPMaterialPropertyBaseColor{Value = new float4(rng.NextFloat3(), 1f),});
            state.EntityManager.AddComponent<HumanWaitForRouteTag>(human);
            //--Uncomment once Routing is done
        }
    }
}
