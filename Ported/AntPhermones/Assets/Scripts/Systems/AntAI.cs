using NUnit.Framework.Internal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial struct AntAI: ISystem 
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ant>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var steeringHandle = SteeringRandomizer.Do();
        var obstacleHandle = ObstacleDetection.Do();
        var pheromoneHandle = PheromoneDetection.Do();
        var resourceHandle = ResourceDetection.Do();
        var dynamicsHandle = DoDynamics(state);


        /*
        foreach (var antPosition in SystemAPI.Query<RefRW<Position>>().WithAll<Ant>())
        {
            var position = antPosition.ValueRW.position;
            position.x += 0.01f;
            antPosition.ValueRW.position = position;
        }
        */
    }
}
