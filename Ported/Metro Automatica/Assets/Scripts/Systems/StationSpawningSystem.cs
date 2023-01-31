using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct StationSpawningSystem : ISystem
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

       // var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        //var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        //var ecb = new EntityCommandBuffer(Allocator.Temp);

        var stations = CollectionHelper.CreateNativeArray<Entity>(config.StationCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.StationPrefab, stations);
        
        var wagons = CollectionHelper.CreateNativeArray<Entity>(config.WagonCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.WagonPrefab, wagons);

        LocalTransform first = new LocalTransform();
        LocalTransform last = new LocalTransform();

        for (int i = 0; i < stations.Length; i++)
        {
            var transform = LocalTransform.FromPosition(i * 90, 0, 0);
          //  transform.Rotation = quaternion.RotateY(math.radians(90));
          state.EntityManager.SetComponentData(stations[i], transform);
            if (i == 0)
            {
                first = transform;
                var firstWagon = transform;
                for (int j = 0; j < wagons.Length ; j++)
                {
                    var wagonTransform = LocalTransform.FromPosition(firstWagon.Position.x, 0, 0);
                    wagonTransform.Rotation = quaternion.RotateY(math.radians(90));
                    state.EntityManager.SetComponentData(wagons[j], wagonTransform);
                    //SystemAPI.SetComponent(SystemAPI.GetComponent<Wagon>(wagons[j]), stations.Last());
                    
                    var tempWagon = SystemAPI.GetComponent<Wagon>(wagons[j]);
                    tempWagon.TrainOffset = firstWagon.Position.x;
                    SystemAPI.SetComponent(wagons[j], tempWagon);
                    //var temp = state.GetComponentLookup<Wagon>(false);
                    //EntityManager.
                    firstWagon.Position.x += 5.2f;
                }
            }

            if (i == stations.Length -1)
            {
                last = transform;
            }
            Debug.Log("First:" + first.Position.x);
            //Debug.Log("Last:" + last.Position.x);
            
        }
        //ecb.Playback(state.EntityManager);
        
        //Debug.Log("Last FIRST:" + last.Position.x);
        foreach (var wagon in SystemAPI.Query<RefRW<Wagon>>())
        {
            Debug.Log("Last LAST:" + last.Position.x);
            //transform.LocalTransform = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
            //wagon.ValueRW.currentDestination
            //state.EntityManager.GetComponentData<LocalTransform>(wagon.Item2);
            float3 finalDestination = new float3(180, 0, 0);
            wagon.ValueRW.currentDestination = last.Position + new float3(wagon.ValueRO.TrainOffset, 0, 0);
            //Debug.Log(first.Position.x);
            //transform.ValueRW.Position.x += 1;
            //float3 startDestination
        }
        
        /*foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<Turret>())
        {
            transform.RotateWorld(rotation);
        }*/
        
        
        
        // This system should only run once at startup. So it disables itself after one update.
        var railSpawn = state.EntityManager.Instantiate(config.RailPrefab);
        
        // need var distance /2 between first and last station in x value (for now)

        var railTransform = LocalTransform.FromPosition(first.Position.x + (last.Position.x - first.Position.x)/2, -1, 0);
        
        // need var distance to calculate scale value

        state.EntityManager.SetComponentData(railSpawn, new PostTransformScale{Value = float3x3.Scale(last.Position.x - first.Position.x,1,1)});

        state.EntityManager.SetComponentData(railSpawn, railTransform);
        
        
    }
}
