using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct MoveToPositionAspect : IAspect
{
    private readonly Entity entity;
    private readonly TransformAspect transformAspect;
    private readonly RefRW<AntMovement> antMovement;

    public void Move(float deltaTime, RefRW<Random> random)
    {
        float3 direction = math.normalize(antMovement.ValueRW.pos - transformAspect.LocalPosition);
        transformAspect.LocalPosition += direction * deltaTime * antMovement.ValueRO.speed;
        
        float targetDistance = 0.1f;
        if (math.distance(transformAspect.LocalPosition, antMovement.ValueRW.pos) < targetDistance)
        {
            antMovement.ValueRW.pos = GetRandomPosition(random);
        }
    }

    public float3 GetRandomPosition(RefRW<Random> randomComponent)
    {
        return new float3(
            randomComponent.ValueRW.random.NextFloat(-15f, 15f), 
            0, 
            randomComponent.ValueRW.random.NextFloat(-15f, 15f));
    }
}
