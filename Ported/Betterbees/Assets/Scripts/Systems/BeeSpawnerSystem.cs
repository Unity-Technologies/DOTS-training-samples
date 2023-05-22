using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
public partial struct BeeSpawnerSystem : ISystem {
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        

        
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var spawner in SystemAPI.Query<SpawnerComponent>())
        {
            for (int i = 0; i < Config.beeCount; i++)
            {
                Entity newBee = state.EntityManager.Instantiate(spawner.beePrefab);
            }
        }
    }
}
