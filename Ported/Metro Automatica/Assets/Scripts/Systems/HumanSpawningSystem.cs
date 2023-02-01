using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
partial struct HumanSpawningSystem : ISystem
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
        /*var config = SystemAPI.GetSingleton<Config>();

        var humans = CollectionHelper.CreateNativeArray<Entity>(config.HumanCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.HumanPrefab, humans);

        List<LocalTransform> stationSpawners = new List<LocalTransform>();
        
        foreach (var station in SystemAPI.Query<RefRW<Station>>())
        {
            Debug.Log("I have found some stations");
            stationSpawners.Add(station.ValueRO.HumanSpawnerLocation);
        }

        foreach (var human in humans)
        {
            int randomStation = 0;//Random.Range(0, config.StationCount);
            var humanTransform = LocalTransform.FromPosition(stationSpawners[randomStation].Position);
            state.EntityManager.SetComponentData(human, humanTransform);
        }*/
    }
}
