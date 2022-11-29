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
        foreach (var (transform, waypoint, target) in 
                 SystemAPI.Query<
                     TransformAspect,
                     Waypoint,
                     TargetPosition> ())
        {
            if (math.distancesq(target.Value, transform.WorldPosition.xz) <= Utility.kStopDistance)
            {
       
            }
        }
    }
}