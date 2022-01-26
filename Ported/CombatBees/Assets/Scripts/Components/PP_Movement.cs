using System.Dynamic;
using Unity.Entities;
using Unity.Mathematics;



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
    public MotionType motionType;
    public float t;             //elapsed time

    //Utility function
    public void GoTo(float3 start, float3 end)
    {
        startLocation = start;
        endLocation = end;
        t = 0.0f;
        timeToTravel = math.distance(start, end) / 10.0f;

    }

    public float3 Progress(float time)
    {
        if (t <= 1)
        {
            t += time / (timeToTravel + math.EPSILON);
        }

        return math.lerp(startLocation, endLocation, t);
    }

    public float3 Progress(float time, MotionType motionType)
    {
        if (t <= 1)
        {
            t += time / (timeToTravel + math.EPSILON);
        }

        float3 trans = math.lerp(startLocation, endLocation, t);

        if (motionType == MotionType.BeeBumble)
        {
            trans.y += math.sin(t * math.PI * 10);
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
            timeToTravel = math.distance(start, end) / 10,
            motionType = MotionType.Linear
        };

        return m;
    }
}