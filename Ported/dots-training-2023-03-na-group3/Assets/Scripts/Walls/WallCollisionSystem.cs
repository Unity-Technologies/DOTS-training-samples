using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial struct WallCollisionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        // Enable if using Default instead of Jobs
       // state.RequireForUpdate<Random>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        #region USING JOBS (TEST)
        NativeArray<float3> wallArray = CollectionHelper.CreateNativeArray<float3>(1000,state.WorldUpdateAllocator);
        int i = 0;
        foreach (var wall in
                 SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<Walls>())
        {
            wallArray[i] = wall.ValueRO.Position;
            i++;
        }
        
        CollisionJob job = new CollisionJob
        {
            wallArray=wallArray,
            dt=SystemAPI.Time.DeltaTime,
        };
        job.ScheduleParallel();

        #endregion

        #region DEFAULT

        //RefRW<Random> random = SystemAPI.GetSingletonRW<Random>();
        // foreach ((MoveToPositionAspect moveToPositionAspect, TransformAspect transformAspect) in SystemAPI
        //              .Query<MoveToPositionAspect, TransformAspect>())
        // {
        //     foreach (var food in
        //              SystemAPI.Query<RefRO<LocalTransform>>()
        //                  .WithAll<Food>())
        //     {
        //         if (math.distance(food.ValueRO.Position, transformAspect.WorldPosition) < 0.2f)
        //         {
        //             var direction = food.ValueRO.Position - transformAspect.WorldPosition;
        //             moveToPositionAspect.Move(SystemAPI.Time.DeltaTime, random,Helper.GetPosition(direction));
        //         }
        //     }
        //     
        //     foreach (var home in
        //              SystemAPI.Query<RefRO<LocalTransform>>()
        //                  .WithAll<Home>())
        //     {
        //         if (math.distance(home.ValueRO.Position, transformAspect.WorldPosition) < 0.2f)
        //         {
        //             var direction = home.ValueRO.Position - transformAspect.WorldPosition;
        //             moveToPositionAspect.Move(SystemAPI.Time.DeltaTime, random,Helper.GetPosition(direction));
        //         }
        //     }
        //     
        //     foreach (var wall in
        //              SystemAPI.Query<RefRO<LocalTransform>>()
        //                  .WithAll<Walls>())
        //     {
        //         if (math.distance(wall.ValueRO.Position, transformAspect.WorldPosition) < 0.1f)
        //         {
        //             var direction = wall.ValueRO.Position - transformAspect.WorldPosition;
        //             moveToPositionAspect.Move(SystemAPI.Time.DeltaTime, random,Helper.GetPosition(direction));
        //         }
        //     }
        // }

        #endregion
    }
}

[BurstCompile]
public partial struct CollisionJob : IJobEntity
{
    [ReadOnly] public NativeArray<float3> wallArray;
    public float dt;

    public void Execute(MoveToPositionAspect moveToPositionAspect,[EntityIndexInQuery] int index)
    {
        for (int i=0;i<wallArray.Length;i++)
        {
            if (math.distance(wallArray[i], moveToPositionAspect.GetPosition()) < 0.1f)
            {
                var direction = wallArray[i] - moveToPositionAspect.GetPosition();
                moveToPositionAspect.Move(dt,Helper.GetPosition(direction,index));
            }
        }
    }
}

[BurstCompile]
public static class Helper
{
    public static float3 GetPosition(float3 direction,int index)
    {
        float signX = direction.x < 0f ? 1f : -1f;
        float signZ= direction.z < 0f ? 1f : -1f;
        int random = Unity.Mathematics.Random.CreateFromIndex((uint)index).NextInt(-60,60);

        float radians = Mathf.Deg2Rad * random;
        float x = signX * Mathf.Cos(radians) - signZ * Mathf.Sin(radians);
        float z = signX * Mathf.Sin(radians) + signZ * Mathf.Cos(radians);

        Vector3 offsetNormalized = new Vector3(x, 0f, z).normalized;

        float3 position = new float3(offsetNormalized.x * 5f, 0, offsetNormalized.z * 5f);
        return position;
    }
}
