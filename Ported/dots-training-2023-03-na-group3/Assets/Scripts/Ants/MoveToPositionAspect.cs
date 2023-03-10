using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct MoveToPositionAspect : IAspect
{
    private readonly TransformAspect _transformAspect;
    private readonly RefRW<AntMovement> _antMovement;

    public void Move(float deltaTime, RefRW<Random> random)
    {
        float3 direction = math.normalize(_antMovement.ValueRW.pos - _transformAspect.LocalPosition);
        _transformAspect.LocalPosition += direction * deltaTime * _antMovement.ValueRO.speed;
       
        // Move randomly
        float targetDistance = random.ValueRO.randomSeed.NextFloat(0.1f, 5f);
        if (math.distance(_transformAspect.LocalPosition, _antMovement.ValueRW.pos) < targetDistance)
        {
            _antMovement.ValueRW.pos = GetRandomPosition(random);
        }
    }
    
    public void Move(float deltaTime, float3 newPos)
    {
        _antMovement.ValueRW.pos = newPos;
        float3 direction = math.normalize(_antMovement.ValueRW.pos - _transformAspect.LocalPosition);
        _transformAspect.LocalPosition += direction * deltaTime * _antMovement.ValueRO.speed;
    }

    public float3 GetPosition()
    {
        return _transformAspect.WorldPosition;
    }

    public float3 GetRandomPosition(RefRW<Random> randomSeed)
    {
        return new float3(
            randomSeed.ValueRW.randomSeed.NextFloat(-5f, 5f), 
            0, 
            randomSeed.ValueRW.randomSeed.NextFloat(-5f, 5f));
    }
}
