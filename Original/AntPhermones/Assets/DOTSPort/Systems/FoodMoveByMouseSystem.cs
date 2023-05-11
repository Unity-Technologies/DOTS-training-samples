using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct FoodMoveByMouseSystem : ISystem, ISystemStartStop
{
    int mapSizeX;
    int mapSizeY;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FoodMoveByMouseExecution>();
        state.RequireForUpdate<FoodData>();
        state.RequireForUpdate<GlobalSettings>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetMouseButtonDown(1))
        {
            float3 pos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
            
            pos.x = Mathf.Clamp(pos.x, 0, mapSizeX);
            pos.y = Mathf.Clamp(pos.y, 0, mapSizeY);
            pos.z = 0;
            
            var entity = SystemAPI.GetSingletonEntity<FoodData>();
            if (state.EntityManager.HasComponent<LocalTransform>(entity))
            {
                var localTransform = state.EntityManager.GetComponentData<LocalTransform>(entity);
                localTransform.Position.x = pos.x;
                localTransform.Position.y = pos.y;
                state.EntityManager.SetComponentData(entity, localTransform);
                
                var foodData = state.EntityManager.GetComponentData<FoodData>(entity);
                foodData.Center = new float2(pos.x, pos.y);
                state.EntityManager.SetComponentData(entity, foodData);
            } 
        }
    }

    public void OnStartRunning(ref SystemState state)
    {
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        mapSizeX = globalSettings.MapSizeX;
        mapSizeY = globalSettings.MapSizeY;
    }

    public void OnStopRunning(ref SystemState state)
    {
        
    }
}
