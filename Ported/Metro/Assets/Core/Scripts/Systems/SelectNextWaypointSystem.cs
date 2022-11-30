using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct SelectNextWaypointSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, waypoint, target, waypointtag) in
                 SystemAPI.Query <
                     TransformAspect,
                     RefRW<Waypoint>,
                     RefRW<TargetPosition>, WaypointMovementTag> ())
        {
            if (math.distancesq(target.ValueRW.Value, transform.WorldPosition.xz) <= Utility.kStopDistance)
            {
                waypoint.ValueRW.WaypointID++;
            }
        }
    }
}