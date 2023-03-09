using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct WallCollisionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Random>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        RefRW<Random> random = SystemAPI.GetSingletonRW<Random>();

        foreach ((MoveToPositionAspect moveToPositionAspect, TransformAspect transformAspect) in SystemAPI
                     .Query<MoveToPositionAspect, TransformAspect>())
        {
            foreach (var food in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<Food>())
            {
                if (math.distance(food.ValueRO.Position, transformAspect.WorldPosition) < 0.2f)
                {
                    var direction = food.ValueRO.Position - transformAspect.WorldPosition;
                    moveToPositionAspect.Move(SystemAPI.Time.DeltaTime, random,Helper.GetPosition(direction));
                }
            }
            
            foreach (var home in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<Home>())
            {
                if (math.distance(home.ValueRO.Position, transformAspect.WorldPosition) < 0.2f)
                {
                    var direction = home.ValueRO.Position - transformAspect.WorldPosition;
                    moveToPositionAspect.Move(SystemAPI.Time.DeltaTime, random,Helper.GetPosition(direction));
                }
            }
            
            foreach (var wall in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<Walls>())
            {
                if (math.distance(wall.ValueRO.Position, transformAspect.WorldPosition) < 0.1f)
                {
                    var direction = wall.ValueRO.Position - transformAspect.WorldPosition;
                    moveToPositionAspect.Move(SystemAPI.Time.DeltaTime, random,Helper.GetPosition(direction));
                }
            }
        }
    }
}

public static class Helper
{
    [BurstCompile]
    public static float3 GetPosition(float3 direction)
    {
        float signX = direction.x < 0f ? 1f : -1f;
        float signZ= direction.z < 0f ? 1f : -1f;
        
        float radians = Mathf.Deg2Rad * UnityEngine.Random.Range(-60,60);
        float x = signX * Mathf.Cos(radians) - signZ * Mathf.Sin(radians);
        float z = signX * Mathf.Sin(radians) + signZ * Mathf.Cos(radians);

        Vector3 offsetNormalized = new Vector3(x, 0f, z).normalized;

        float3 position = new float3(offsetNormalized.x * 5f, 0, offsetNormalized.z * 5f);
        return position;
    }
}
