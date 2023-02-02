using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct PierreDebug_QueueInputSystem : ISystem
{
    Random m_Random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_Random = Random.CreateFromIndex(1234);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var doorTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Door>())
            {
                doorTransform.ValueRW.Scale = doorTransform.ValueRW.Scale > 0 ? 0 : 1;
            }

            foreach (var train in SystemAPI.Query<RefRW<Train>>())
            {
                train.ValueRW.State = train.ValueRW.State == TrainState.Idle ? TrainState.Boarding : TrainState.Idle;
            }
        }
    }
}
