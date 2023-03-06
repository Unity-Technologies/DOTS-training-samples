using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[UpdateAfter(typeof(ObstacleSpawnerSystem))]
//[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct PlayerSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<ExecutePlayerSpawner>();
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // We only want to spawn cars in one frame. Disabling the system stops it from updating again after this one time.
        state.Enabled = false;

        var config = SystemAPI.GetSingleton<Config>();
        var random = new Unity.Mathematics.Random(10101);

        for (int Idx = 0; Idx < config.NumCars; ++Idx)
        {
            var car = state.EntityManager.Instantiate(config.CarPrefab);

            // Set the new player's transform (a position offset from the obstacle).
            state.EntityManager.SetComponentData(car, new LocalTransform
            {
                Position = new float3
                {
                    x = random.NextFloat(10),
                    y = 1,
                    z = random.NextFloat(10),
                },
                Scale = 1,  // If we didn't set Scale and Rotation, they would default to zero (which is bad!)
                Rotation = quaternion.identity
            });
        }
    }
}