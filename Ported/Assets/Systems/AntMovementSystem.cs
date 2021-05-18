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
        const float maxDirectionChangePerSecond = 5.0f;

        var random = new Random((uint)(Time.ElapsedTime * 1000f + 1f));
        var deltaTime = Time.DeltaTime;
        var simulationSpeedEntity = GetSingletonEntity<SimulationSpeed>();
        var simulationSpeed = GetComponent<SimulationSpeed>(simulationSpeedEntity).Value;

        Entities
            .WithAll<Ant>()
            .ForEach((Entity entity, ref Translation translation, ref Direction direction, ref Rotation rotation) =>
            {
                var delta = new float3(Mathf.Cos(direction.Radians), Mathf.Sin(direction.Radians), 0);
                translation.Value += delta * deltaTime * simulationSpeed;

                direction.Radians += random.NextFloat(-maxDirectionChangePerSecond * deltaTime * simulationSpeed,
                                                       maxDirectionChangePerSecond * deltaTime * simulationSpeed);
                
                rotation = new Rotation {Value = quaternion.RotateZ(direction.Radians)};
            }).Schedule();
    }
}