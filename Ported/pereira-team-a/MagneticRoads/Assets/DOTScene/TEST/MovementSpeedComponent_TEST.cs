
using System;
using Unity.Entities;
using Unity.Mathematics;


[Serializable]
public struct MovementSpeedComponent_TEST : IComponentData
{
    public float speed;
    public float3 lastPosition;
    public int currentTrackSpline;
    public int lastTrackSpline;
    public int dir;
    public float3 forward;
    public bool init;
    public int next; //Just for debug
}
