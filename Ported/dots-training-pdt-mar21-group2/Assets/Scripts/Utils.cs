using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

struct Utils
{
    static public bool WorldIsOutOfBounds(Unity.Mathematics.float3 position, float width, float ground)
    {
        return position.x < 0.0f ||
                position.x > width ||
                position.y < ground;
    }
    
    static public bool FindNearestRock(
        Translation armTranslation,
        NativeArray<Entity> availableRocks, 
        ComponentDataFromEntity<Translation> translations, 
        out Entity nearestRock)
    {
        const float grabDist = 6.0f;
        const float grabDistSq = grabDist * grabDist;
        
        foreach(var rockEntity in availableRocks)
        {
            var rockTranslation = translations[rockEntity];
            var distSq = math.distancesq(armTranslation.Value, rockTranslation.Value);

            if (distSq < grabDistSq)
            {
                nearestRock = rockEntity;
                return true;
            }
        }

        nearestRock = default;
        return false;
    }

    static public quaternion FromToRotation(float3 from, float3 to)
    {
        float3 half = math.normalize(from + to);
        var xyz = math.cross(from, half);
        var w = math.dot(from, half);

        return new quaternion(xyz.x, xyz.y, xyz.z, w);
    }
    
    static public void GoToState<TFromState, TToState>(EntityCommandBuffer ecb, 
        Entity entity,
        float duration = 1.0f)
        where TFromState : unmanaged, IComponentData
        where TToState : unmanaged, IComponentData
    {
        ecb.RemoveComponent<TFromState>(entity);
        ecb.AddComponent(entity, new TToState());
        
        ecb.SetComponent(entity, new Timer() {Value = duration});
        ecb.SetComponent(entity, new TimerDuration() {Value = duration}); 
    }
    
    static public void GoToState<TFromState, TToState>(EntityCommandBuffer.ParallelWriter ecb, 
        int entityInQueryIndex,
        Entity entity,
        float duration = 1.0f)
        where TFromState : unmanaged, IComponentData
        where TToState : unmanaged, IComponentData
    {
        ecb.RemoveComponent<TFromState>(entityInQueryIndex, entity);
        ecb.AddComponent(entityInQueryIndex, entity, new TToState());
        
        ecb.SetComponent(entityInQueryIndex, entity, new Timer() {Value = duration});
        ecb.SetComponent(entityInQueryIndex, entity, new TimerDuration() {Value = duration});
    }

    static public bool DidAnimJustStarted(Timer timer, TimerDuration timerDuration)
    {
        return timer.Value >= timerDuration.Value;
    }
    
    static public bool DidAnimJustFinished(Timer timer)
    {
        return timer.Value <= 0.0f;
    }
    
    static public bool IsPlayingAnimation(Timer timer)
    {
        return timer.Value > 0.0f;
    }
}