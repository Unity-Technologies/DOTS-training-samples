using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct ScaleDownSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }
    
    public void OnDestroy(ref SystemState state)
    {
       
    }
    
    public void OnUpdate(ref SystemState state)
    {
        ScaleDown job = new ScaleDown{deltaTime = state.Time.DeltaTime};
        job.Schedule();
    }
}

[BurstCompile]
public partial struct ScaleDown : IJobEntity
{
    public float deltaTime;
    public void Execute(ref NonUniformScale scale ,ref TransformAspect transform,in Health health)
    {
        float3 newScale = scale.Value;
        float3 newPosition = transform.Position;
        newPosition.y -= scale.Value.y * 0.5f;
        newScale.y = health.Value;
        newPosition.y += newScale.y * 0.5f;
        scale.Value = newScale;
        transform.Position = newPosition;
    }
}