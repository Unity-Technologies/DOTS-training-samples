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
        }
   
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);      
            float3 planeFloor = new float3(0, boundsComponent.Center.y - boundsComponent.Extents.y, 0);
            float3 planeCeil = new float3(0, boundsComponent.Center.y + boundsComponent.Extents.y, 0);

            float3 lateraltWall1 = new float3(0, 0, boundsComponent.Center.z - boundsComponent.Extents.z);
            float3 lateraltWall2 = new float3(0, 0, boundsComponent.Center.z + boundsComponent.Extents.z);

            float3 capWall1 = new float3(arenaBounds.Value.Center.x - arenaBounds.Value.Extents.x, 0, 0);
            float3 capWall2 = new float3(boundsComponent.Center.x + boundsComponent.Extents.x, 0, 0);

            
            if (new Plane(Vector3.up, planeFloor + 1f).Raycast(ray, out var hit))
            {
                if (Vector3.Dot(ray.direction, Vector3.up) < 0 && arenaBounds.Value.Contains(ray.GetPoint(hit)))
                {
                    var foodEntity = commandBuffer.Instantiate(gameConfig.FoodPrefab);
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = ray.GetPoint(hit) });
                    commandBuffer.AddComponent(foodEntity, new Force() { });
                    commandBuffer.AddComponent(foodEntity, new Velocity() { });
                }
            }
            if (new Plane(Vector3.up, planeCeil - 1f).Raycast(ray, out var hit2))
            {
                if (Vector3.Dot(ray.direction, Vector3.down) < 0 && arenaBounds.Value.Contains(ray.GetPoint(hit2)))
                {
                    var foodEntity = commandBuffer.Instantiate(gameConfig.FoodPrefab);
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = ray.GetPoint(hit2) });
                    commandBuffer.AddComponent(foodEntity, new Force() { });
                    commandBuffer.AddComponent(foodEntity, new Velocity() { });
                }
            }
            if (new Plane(new float3(0, 0, 1), lateraltWall1 + 1f).Raycast(ray, out var hit3))
            {
                if (Vector3.Dot(ray.direction, new float3(0, 0, 1)) < 0 && arenaBounds.Value.Contains(ray.GetPoint(hit3)))
                {
                    var foodEntity = commandBuffer.Instantiate(gameConfig.FoodPrefab);
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = ray.GetPoint(hit3) });
                    commandBuffer.AddComponent(foodEntity, new Force() { });
                    commandBuffer.AddComponent(foodEntity, new Velocity() { });
                }
            }
            if (new Plane(new float3(0, 0, -1), lateraltWall2 - 1f).Raycast(ray, out var hit4))
            {
                if (Vector3.Dot(ray.direction, new float3(0, 0, -1)) < 0 && arenaBounds.Value.Contains(ray.GetPoint(hit4)))
                {
                    var foodEntity = commandBuffer.Instantiate(gameConfig.FoodPrefab);
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = ray.GetPoint(hit4) });
                    commandBuffer.AddComponent(foodEntity, new Force() { });
                    commandBuffer.AddComponent(foodEntity, new Velocity() { });
                }
            }
            if (new Plane(new float3(1, 0, 0), capWall1 + 1f).Raycast(ray, out var hit5))
            {
                if (Vector3.Dot(ray.direction, new float3(1, 0, 0)) < 0 && arenaBounds.Value.Contains(ray.GetPoint(hit5)))
                {
                    var foodEntity = commandBuffer.Instantiate(gameConfig.FoodPrefab);
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = ray.GetPoint(hit5) });
                    commandBuffer.AddComponent(foodEntity, new Force() { });
                    commandBuffer.AddComponent(foodEntity, new Velocity() { });
                }
            }
            if (new Plane(new float3(-1, 0, 0), capWall2 - 1f).Raycast(ray, out var hit6))
            {
                if (Vector3.Dot(ray.direction, new float3(-1, 0, 0)) < 0 && arenaBounds.Value.Contains(ray.GetPoint(hit6)))
                {
                    var foodEntity = commandBuffer.Instantiate(gameConfig.FoodPrefab);
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = ray.GetPoint(hit6) });
                    commandBuffer.AddComponent(foodEntity, new Force() { });
                    commandBuffer.AddComponent(foodEntity, new Velocity() { });
                }
            }
            
        }
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}