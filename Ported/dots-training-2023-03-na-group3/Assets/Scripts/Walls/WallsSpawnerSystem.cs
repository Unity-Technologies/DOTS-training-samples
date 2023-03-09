using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct WallsSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var food = SystemAPI.GetSingleton<Food>();
        var home = SystemAPI.GetSingleton<Home>();
        
        var foodEntity = state.EntityManager.Instantiate(food.foodPrefab);
        var homeEntity = state.EntityManager.Instantiate(home.homePrefab);

        var positions = new[]
        {
            new float3(3.5f, 0, 3.5f),
            new float3(-3.5f, 0, 3.5f),
            new float3(-3.5f, 0, -3.5f),
            new float3(3.5f, 0, -3.5f)
        };
        
        state.EntityManager.SetComponentData(foodEntity, new LocalTransform
        {
            Position = positions[UnityEngine.Random.Range(0,4)],
            Rotation = quaternion.identity,
            Scale = 0.4f
        });
        
        state.EntityManager.SetComponentData(homeEntity, new LocalTransform
        {
            Position = float3.zero,
            Rotation = quaternion.identity,
            Scale = 0.4f
        });

        foreach (WallsSpawner spawner in SystemAPI.Query<WallsSpawner>())
        {
            var numOpenings = UnityEngine.Random.Range(1, 3);
            switch (numOpenings)
            {
                case 1:
                {
                    var r = UnityEngine.Random.Range(0, 360);
                    var arcLength = UnityEngine.Random.Range(220,270);
                    var startAngle = r; 
                    var endAngle = r+arcLength;
                    DrawArc(startAngle,endAngle,spawner,state);
                    break;
                }
                case 2:
                {
                    var r = UnityEngine.Random.Range(0, 360);
                    var arcLength = 120;
                    var startAngle = r; 
                    var endAngle = r+arcLength;
                    var gap = 60;
                
                    // Top arc
                    DrawArc(startAngle,endAngle,spawner,state);

                    // Bottom arc
                    DrawArc(endAngle+gap,endAngle+gap+arcLength,spawner,state);
                    break;
                }
            }
        }
        state.Enabled = false;
    }

    [BurstCompile]
    private void DrawArc(int startAngle, int endAngle, WallsSpawner spawner, SystemState state)
    {
        var radius = spawner.radius;
        var numSpheres = spawner.numSpheres;
        for (var i = 0; i < numSpheres; i++)
        {
            var angleBetweenObjects = (endAngle - startAngle) / (numSpheres - 1);
            var angle = startAngle + i * angleBetweenObjects;
            var x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            var y = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
            var position = new float3(x, spawner.position.z, y);
                 
            var sphereEntity = state.EntityManager.Instantiate(spawner.environmentPrefab);
            state.EntityManager.SetComponentData(sphereEntity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 0.2f
            });
        }
    }
}