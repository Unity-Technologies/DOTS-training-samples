using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct AntSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntSpawner>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var antSpawnerComponent = SystemAPI.GetSingleton<AntSpawner>();

        for (var i = 0; i < antSpawnerComponent.antAmount; i++)
        {
            state.EntityManager.Instantiate(antSpawnerComponent.antPrefab);
        }
    }
}
