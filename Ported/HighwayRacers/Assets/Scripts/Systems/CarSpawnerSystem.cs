using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CarSpawnerSystem : ISystem
{
    
    
    
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CarSpawner>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var spawner = SystemAPI.GetSingleton<CarSpawner>();

        if (!spawner.spawned)
        {
            InitialSpawn(ref spawner, ref state);
        }

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     var enemyEntity = state.EntityManager.Instantiate(spawner.carPrefab);
        //     var position = new float3 {x = Random.Range(25f, 50f), y = Random.Range(25f, 50f), z = Random.Range(25f, 50f)};
        //     InitEntity(enemyEntity, position, ref state);
        // }

        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     //DeleteRandom(ref state);
        // }
    }

    private void InitialSpawn(ref CarSpawner spawner, ref SystemState state)
    {
        var carEntities = new NativeArray<Entity>(spawner.amount, Allocator.Temp);
        state.EntityManager.Instantiate(spawner.carPrefab, carEntities);
        // EntityBehaviourManager.Instance.Positions = new TransformAccessArray(spawner.amount);

        for (var i = 0; i < carEntities.Length; i++)
        {

            int lane = Random.Range(0, spawner.NumLanes);
            var carEntity = carEntities[i];
            //TODO: position calculation on track
            var position = new float3 {x = (float)lane, y = 0.0f, z = Random.Range(0.0f, spawner.LengthLanes)};
            
            state.EntityManager.SetComponentData(carEntity, new LocalTransform {Position = position, Scale = 1});
        
            state.EntityManager.AddComponentData(carEntity, new CarVelocity {VelX = 0.0f, VelY = Random.Range(spawner.MinVelocity, spawner.MaxVelocity)});
            state.EntityManager.AddComponentData(carEntity, new CarOvertakeState {OvertakeStartTime = 0.0f, OriginalLane = lane, TargetLane = lane});
            state.EntityManager.AddComponentData(carEntity, new CarIsOvertaking {IsOvertaking = false});
            state.EntityManager.AddComponentData(carEntity, new CarAdjacency{Left = false, Right = false, Front = false});
            
        }

        spawner.spawned = true;
        SystemAPI.SetSingleton(spawner);
        
        
        
    }

    private void InitEntity(Entity carEntity, float3 position, ref SystemState state)
    {
        

        
        
        
    }

    // private void DeleteRandom(ref SystemState state)
    // {
    //     var indexToDelete = Random.Range(0, EntityBehaviourManager.Instance.All.Count);
    //     var entityToDestroy = EntityBehaviourManager.Instance.All[indexToDelete];
    //     state.EntityManager.DestroyEntity(entityToDestroy.Entity);
    //     //entityToDestroy.Destroy(); Also possible
    // }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}