using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class InputSystem : SystemBase
{
    private EntityQuery boundsQuery;
    private EntityQuery allWithForceQuery;
    private EntityQuery arenaQuery;

    protected override void OnCreate()
    { 
        var boundsQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(SpawnBounds) },
        };
        boundsQuery = GetEntityQuery(boundsQueryDesc);

        var allWithForce = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Force) },
        };
        allWithForceQuery = GetEntityQuery(allWithForce);

        var getArena = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(GameConfiguration), typeof(WorldRenderBounds)}
        };
        arenaQuery = GetEntityQuery(getArena);
    }

    protected override void OnUpdate()
    {
        var spawnBoundsArray = boundsQuery.ToEntityArray(Allocator.Temp);
        var arena = arenaQuery.ToEntityArray(Allocator.Temp);
        var gameConfig = GetSingleton<GameConfiguration>();
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var boundsComponent = GetComponent<SpawnBounds>(spawnBoundsArray[0]);
        var arenaBounds = GetComponent<WorldRenderBounds>(arena[0]);
    
        if (Input.GetKeyDown(KeyCode.R))
        {
            commandBuffer.DestroyEntitiesForEntityQuery(allWithForceQuery);
            var initConfig = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(initConfig, new InitialSpawnConfiguration() { BeeCount = gameConfig.BeeCount, FoodCount = gameConfig.FoodCount });
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
   
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);      
            float3 planeFloor = new float3(0, boundsComponent.Center.y - boundsComponent.Extents.y, 0);
            float3 planeCeil= new float3(0, boundsComponent.Center.y + boundsComponent.Extents.y, 0);

            var foodEntity = commandBuffer.Instantiate(gameConfig.FoodPrefab);
            if (new Plane(Vector3.up, planeFloor + 1f).Raycast(ray, out var hit))
            {
                if (Vector3.Dot(ray.direction, Vector3.up) < 0 && arenaBounds.Value.Contains(ray.GetPoint(hit)))
                {                    
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = ray.GetPoint(hit) });
                    commandBuffer.AddComponent(foodEntity, new Force() { });
                    commandBuffer.AddComponent(foodEntity, new Velocity() { });
                    commandBuffer.Playback(EntityManager);
                    commandBuffer.Dispose();
                }
            }
            if (new Plane(Vector3.up, planeCeil - 1f).Raycast(ray, out var hit2))
            {
                if (Vector3.Dot(ray.direction, Vector3.down) < 0 && arenaBounds.Value.Contains(ray.GetPoint(hit2)))
                {
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = ray.GetPoint(hit2) });
                    commandBuffer.AddComponent(foodEntity, new Force() { });
                    commandBuffer.AddComponent(foodEntity, new Velocity() { });
                    commandBuffer.Playback(EntityManager);
                    commandBuffer.Dispose();
                }
            }
        }   
    }
}