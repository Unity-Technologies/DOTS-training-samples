using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;


public enum MotionType
{
    Linear,
    BeeBumble,
    FoodDangle,
}

[GenerateAuthoringComponent]
public struct PP_Movement : IComponentData
{
    public float3 startLocation;
    public float3 endLocation;
    public float timeToTravel;
    public float unupdatedTimeToTravel;
    public float3 unperturbedPosition;
    public float timeDilation;
    public float t; //elapsed time

    //Utility function
    public void GoTo(float3 start, float3 end)
    {
        unperturbedPosition = startLocation = start;
        endLocation = end;
        unupdatedTimeToTravel = timeToTravel = distance(start, end) * 0.1f;
        timeDilation = 1f;
        t = 0.0f;
    }

    public void UpdateEndPosition(float3 end)
    {
        endLocation = end;
        var newTimeToTravel = distance(startLocation, end) * 0.1f;

        // Need to adjust progression speed faster when the distance has shortened,
        // and slower when it has lengthened.
        timeDilation = unupdatedTimeToTravel / newTimeToTravel;

        timeToTravel = newTimeToTravel;
    }

    public float3 Progress(float time, MotionType motionType)
    {
        if (t <= 1)
        {
            t += time / (timeToTravel + EPSILON) * timeDilation;
        }

        t = math.clamp(t, 0f, 1f);

        float3 trans = unperturbedPosition = lerp(startLocation, endLocation, t);

        if (motionType == MotionType.BeeBumble)
        {
            trans.y += GetBumbledHeightWithFade(t, timeToTravel);
        }

        return trans;
    }

    public float3 GetTransAtProgress(float futureT, MotionType motionType)
    {
        float3 trans = lerp(startLocation, endLocation, futureT);

        if (motionType == MotionType.BeeBumble)
        {
            trans.y += GetBumbledHeightWithFade(futureT, timeToTravel);
        }

        return trans;
    }

    public static PP_Movement Create(float3 start, float3 end)
    {
        var m = new PP_Movement
        {
            startLocation = start,
            unperturbedPosition = start,
            endLocation = end,
            timeToTravel = distance(start, end) * 0.1f,
            unupdatedTimeToTravel = distance(start, end) * 0.1f,
            timeDilation = 1f,
            t = 0.0f
        };

        return m;
    }

    private static float GetBumbledHeightWithFade(float t, float timeToTravel)
    {
        if (t >= 1f) return 1f;
        if (timeToTravel <= 0) return 0f;

        var distanceToTarget = timeToTravel * 10f;

        // Pure Bumble
        var bounceCount = distanceToTarget / 6f;
        var bumbledPosition = abs(sin(t * PI * bounceCount));

        // We need to fade out the end of the movement to ensure the last position is at the target.
        // Fade over the last "bounce" of the bumble
        var fadeScalar = min(distanceToTarget, 6f) / distanceToTarget;
        fadeScalar += clamp((t - 1f + fadeScalar) / fadeScalar, 0f, 1f);
        
        return lerp(bumbledPosition, 0, fadeScalar);
    }
}