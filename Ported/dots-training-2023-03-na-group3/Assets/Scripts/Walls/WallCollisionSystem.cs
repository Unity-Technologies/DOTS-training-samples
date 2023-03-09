using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct WallCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        RefRW<Random> random = SystemAPI.GetSingletonRW<Random>();

        foreach ((MoveToPositionAspect moveToPositionAspect, TransformAspect transformAspect) in SystemAPI
                     .Query<MoveToPositionAspect, TransformAspect>())
        {
            foreach (var wall in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<Walls>())
            {
                if (math.distance(wall.ValueRO.Position, transformAspect.WorldPosition) < 0.1f)
                {
                    moveToPositionAspect.Move(SystemAPI.Time.DeltaTime, random,GetRandomPosition(random));
                }
            }
        }
    }
    
    [BurstCompile]
    public float3 GetRandomPosition(RefRW<Random> randomSeed)
    {
        return new float3(
            randomSeed.ValueRW.randomSeed.NextFloat(-5f, 5f), 
            0, 
            randomSeed.ValueRW.randomSeed.NextFloat(-5f, 5f));
    }
}