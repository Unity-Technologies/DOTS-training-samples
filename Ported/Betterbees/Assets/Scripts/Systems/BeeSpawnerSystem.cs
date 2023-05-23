using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct BeeSpawnerSystem : ISystem
{
    private uint _updateCounter;

    private bool haveBeesSpawned;
    
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
        if (haveBeesSpawned)
        {
            return;
        }

        var config = SystemAPI.GetSingleton<Config>();
        var random = Random.CreateFromIndex(_updateCounter++);

        foreach (var spawner in SystemAPI.Query<SpawnerComponent>())
        {
            for (int i = 0; i < config.beeCount; i++)
            {
                Entity newBee = state.EntityManager.Instantiate(spawner.beePrefab);
                state.EntityManager.SetComponentData(newBee, new VelocityComponent
                {
                    Velocity = random.NextFloat3Direction() * config.maxSpawnSpeed
                });
            }
        }

        haveBeesSpawned = true;

        // spawn new bees when food is placed in hive
        // remove food that has been placed
    }
}
