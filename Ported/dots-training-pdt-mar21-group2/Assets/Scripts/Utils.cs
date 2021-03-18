using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

struct Utils
{
    public static bool WorldIsOutOfBounds(Unity.Mathematics.float3 position, float width, float ground)
    {
        return position.x < 0.0f ||
               position.x > width ||
               position.y < ground;
    }

    public static bool FindNearestRock(
        Translation armTranslation,
        NativeArray<Entity> availableRocks,
        ComponentDataFromEntity<Translation> translations,
        out Entity nearestRock)
    {
        const float grabDist = 6.0f;
        const float grabDistSq = grabDist * grabDist;

        foreach (var rockEntity in availableRocks)
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

    public static bool FindNearestCan(
        Translation armTranslation,
        NativeArray<Entity> availableCans,
        ComponentDataFromEntity<Translation> translations,
        out Entity nearestCan)
    {
        bool retVal = false;
        nearestCan = default;
        float curentDistance = float.MaxValue;
        foreach (var canEntity in availableCans)
        {
            var canTranslation = translations[canEntity];
            var distSq = math.distancesq(armTranslation.Value, canTranslation.Value);


            if (distSq < curentDistance)
            {
                nearestCan = canEntity;
                retVal = true;
            }
        }

        return retVal;
    }

    static public quaternion FromToRotation(float3 from, float3 to)
    {
        float3 half = from + to;
        if (math.lengthsq(half) < 0.01f)
        {
            return new quaternion(1.0f, 0.0f, 0.0f, 0.0f);
        }
        
        half = math.normalize(half);
        var xyz = math.cross(from, half);
        var w = math.dot(from, half);

        var rot = new quaternion(xyz.x, xyz.y, xyz.z, w);
        rot.value = math.normalize(rot.value);
        return rot;
    }

    public static void GoToState<TFromState, TToState>(EntityCommandBuffer ecb,
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

    public static void GoToState<TFromState, TToState>(EntityCommandBuffer.ParallelWriter ecb,
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

    public static bool DidAnimJustStarted(Timer timer, TimerDuration timerDuration)
    {
        return timer.Value >= timerDuration.Value;
    }

    public static bool DidAnimJustFinished(Timer timer)
    {
        return timer.Value <= 0.0f;
    }

    public static bool IsPlayingAnimation(Timer timer)
    {
        return timer.Value > 0.0f;
    }

    public static uint GetArmCount(SystemBase system)
    {
        var worldBounds = system.GetSingleton<WorldBounds>();
        var parameters = system.GetSingleton<SimulationParameters>();
        
        // add margin to make sure arm won't try to grab for rock that will be recycled soon
        var margin = parameters.ArmSeparation * 4.0f;

        return (uint) ((worldBounds.Width - margin) / parameters.ArmSeparation);
    }

    public static float GetArmRowWidth(SystemBase system)
    {
        return system.GetSingleton<SimulationParameters>().ArmSeparation * GetArmCount(system);
    }
}
