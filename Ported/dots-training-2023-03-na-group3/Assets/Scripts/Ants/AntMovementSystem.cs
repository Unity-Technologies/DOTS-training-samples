using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

public partial struct AntMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Random>();
        state.RequireForUpdate<PheromoneData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var pheromones = SystemAPI.GetSingletonBuffer<PheromoneData>();
        RefRW<Random> random = SystemAPI.GetSingletonRW<Random>();

        foreach (MoveToPositionAspect moveToPositionAspect in SystemAPI.Query<MoveToPositionAspect>())
        {
            moveToPositionAspect.Move(SystemAPI.Time.DeltaTime, random);
        }
    }
    float PheromoneSteering(float2 antposition,float distance, float antangle, int mapSize, DynamicBuffer<PheromoneData> pheromones) {
        float output = 0;

        for (int i=-1;i<=1;i+=2) {
            float angle = antangle + i * math.PI*.25f;
            float testX = antposition.x + math.cos(angle) * distance;
            float testY = antposition.y + math.sin(angle) * distance;

            if (testX <0 || testY<0 || testX>=mapSize || testY>=mapSize) {

            } else
            {
                int index = (int)testX + (int)testY * mapSize;
                float value = pheromones[index].value;
                output += value*i;
            }
        }
        return math.sign(output);
    }
    
}
