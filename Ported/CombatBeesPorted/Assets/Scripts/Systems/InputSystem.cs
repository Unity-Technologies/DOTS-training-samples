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
    }

    protected override void OnUpdate()
    {
        var spawnBoundsArray = boundsQuery.ToEntityArray(Allocator.Temp);
        var gameConfig = GetSingleton<GameConfiguration>();
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var boundsComponent = GetComponent<SpawnBounds>(spawnBoundsArray[0]);
    
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
            if (new Plane(Vector3.up, planeFloor + 1f).Raycast(ray, out var hit))
            {
                var foodEntity = commandBuffer.Instantiate(gameConfig.FoodPrefab);
                if (GetComponent<SpawnBounds>(spawnBoundsArray[0]).ContainsPoint( ray.GetPoint(hit)) ||
                    GetComponent<SpawnBounds>(spawnBoundsArray[1]).ContainsPoint(ray.GetPoint(hit)))
                {                    
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = ray.GetPoint(hit) });
                    commandBuffer.AddComponent(foodEntity, new Force() { });
                    commandBuffer.AddComponent(foodEntity, new Velocity() { });
                    commandBuffer.Playback(EntityManager);
                    commandBuffer.Dispose();

                } 
            }   
        }

        
    }
}