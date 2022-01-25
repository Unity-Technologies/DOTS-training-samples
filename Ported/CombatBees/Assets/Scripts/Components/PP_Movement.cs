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
}