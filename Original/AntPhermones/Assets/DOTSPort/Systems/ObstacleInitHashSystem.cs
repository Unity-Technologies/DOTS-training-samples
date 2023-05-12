using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
public partial struct ObstacleInitHashSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<CollisionHashSet>();
        state.RequireForUpdate<ObstacleArcPrimitive>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();

        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponent<CollisionHashSet>(entity);
        
        var hashSet = state.EntityManager.GetComponentData<CollisionHashSet>(entity);//SystemAPI.GetSingleton<CollisionHashSet>();
        var collidersNativeArray = SystemAPI.GetSingletonBuffer<ObstacleArcPrimitive>().AsNativeArray();

        hashSet.CollisionSet = new NativeHashSet<int2>(globalSettings.MapSizeX * globalSettings.MapSizeY, Allocator.Persistent);

        var wallThickness = globalSettings.WallThickness * 3f;
        float mapSizeX = globalSettings.MapSizeX;
        float mapSizeY = globalSettings.MapSizeY;
        
        Debug.Log($"collidersNativeArray: {collidersNativeArray.Length}");
        
        for (int x = 0; x < globalSettings.MapSizeX; x++)
        {
            for (int y = 0; y < globalSettings.MapSizeY; y++)
            {
                var coord = new int2(x, y);
                if (ObstacleSpawnerSystem.IsGridOccupied(collidersNativeArray, new float2(x / mapSizeX, y / mapSizeY), wallThickness))
                {
                    hashSet.CollisionSet.Add(coord);
                }
            }
        }
        
        state.EntityManager.SetComponentData(entity, hashSet);

        Debug.Log($"hashSet size: {hashSet.CollisionSet.Count}");
    }

    public void OnDestroy(ref SystemState state)
    {
        var hashSet = SystemAPI.GetSingleton<CollisionHashSet>();
        hashSet.CollisionSet.Dispose();
    }
}


public struct CollisionHashSet: IComponentData
{
    public NativeHashSet<int2> CollisionSet;
}
