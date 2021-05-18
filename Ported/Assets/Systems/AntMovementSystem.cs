using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;


public class AntMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Random(1234);
        var deltaTime = Time.DeltaTime;
        var simulationSpeedEntity = GetSingletonEntity<SimulationSpeed>();
        var simulationSpeed = GetComponent<SimulationSpeed>(simulationSpeedEntity).Value;

        Entities
            .WithAll<Ant>()
            .ForEach((Entity entity, ref Translation translation, in Direction direction) =>
            {
                var delta = new float3(Mathf.Cos(direction.Radians), Mathf.Sin(direction.Radians), 0);
                translation.Value += delta * deltaTime * simulationSpeed;
            }).Schedule();
    }
}