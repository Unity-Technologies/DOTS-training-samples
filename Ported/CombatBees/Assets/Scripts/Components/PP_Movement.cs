using System;
using Unity.Entities;
using Unity.Mathematics;
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
    public float startTime;
    public float timeToTravel;
    public float t;             //elapsed time

    //Utility function
    public void GoTo(float3 start, float3 end)
    {
        startLocation = start;
        endLocation = end;
        t = 0.0f;
        timeToTravel = distance(start, end) * 0.1f;

    }

    public float3 Progress(float time)
    {
        if (t <= 1)
        {
            t += time / (timeToTravel + EPSILON);
        }

        return lerp(startLocation, endLocation, t);
    }

    public float3 Progress(float time, MotionType motionType)
    {
        if (t <= 1)
        {
            t += time / (timeToTravel + EPSILON);
        }

        t = math.clamp(t, 0f, 1f);

        float3 trans = lerp(startLocation, endLocation, t);

        if (motionType == MotionType.BeeBumble)
        {
            int bounceCount = (int)round((timeToTravel * 10f / 6f));
            trans.y += abs(sin(t * PI * bounceCount));
        }

        return trans;
    }

    public float3 GetTransAtProgress(float futureT, MotionType motionType)
    {
        float3 trans = lerp(startLocation, endLocation, futureT);

        if (motionType == MotionType.BeeBumble)
        {
            int bounceCount = (int)round((timeToTravel * 10f / 6f));
            trans.y += abs(sin(futureT * PI * bounceCount));
        }

        return trans;
    }

    public static PP_Movement Create(float3 start, float3 end)
    {
        var m = new PP_Movement
        {
            startLocation = start,
            endLocation = end,
            t = 0.0f,
            timeToTravel = distance(start, end) / 10,
        };

        return m;
    }
}