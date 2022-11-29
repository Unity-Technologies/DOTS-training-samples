using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

[UpdateAfter(typeof(AntMovementSystem))]
partial struct AntUnspawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }


    public void OnDestroy(ref SystemState state)
    {
    }


    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (transform, entity) in SystemAPI.Query<TransformAspect>().WithAll<Ant>().WithEntityAccess())
        {
            float halfSize = config.PlaySize * 0.5f;
            if (math.abs(transform.WorldPosition.x) > halfSize || math.abs(transform.WorldPosition.z) > halfSize)
            {
                ecb.DestroyEntity(entity);
            }
        }

    }
}
