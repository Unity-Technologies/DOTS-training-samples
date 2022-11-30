using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TrainAuthoring : UnityEngine.MonoBehaviour
{
    public float MaxSpeed;
    public float StopTime;
    public Transform InitWaypoint;
}
 
class TrainBaker : Baker<TrainAuthoring>
{
    public override void Bake(TrainAuthoring authoring)
    {
        /*
        AddComponent(new IdleTime
        {
            Value = authoring.StopTime
        });
        */
        AddComponent(new Speed
        {
            Value = authoring.MaxSpeed
        });
        AddComponent(new Position
        {
            Value = new float2(0f, 0f)
        });
        AddComponent(new Direction
        {
            Value = new float2(0f, 1f)
        });
        
        var initTarget = authoring.InitWaypoint.position;
        AddComponent(new TargetPosition
        {
            Value = new float2(initTarget.x, initTarget.z)
        });
        //AddComponent<Waypoint>();
        
        AddComponent(new Agent
        {
            CurrentWaypoint = GetEntity(authoring.InitWaypoint)
        });
        
    }
}