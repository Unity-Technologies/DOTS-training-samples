using System.Dynamic;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PP_Movement : IComponentData
{
    public float3 startLocation;
    public float3 endLocation;
    public float startTime;
    public float timeToTravel;
    public float t;             //elapsed time
                                //currently the travel time should be equivalent to the end-start

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

    public static PP_Movement Create(float3 start, float3 end)
    {
        var m = new PP_Movement
        {
            startLocation = start,
            endLocation = end,
            t = 0.0f,
            timeToTravel = math.distance(start, end) / 10
        };

        return m;
    }
}