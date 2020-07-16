using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CommuterWalkSystem : SystemBase
{
    protected override void OnUpdate()
    {
        const float speed = 0.5f;
        var deltaTime = Time.DeltaTime;
        Entities
            .ForEach((ref Commuter commuter, ref Translation translation, in DynamicBuffer<CommuterWaypoint> waypointsBuffer) =>
        {
            var targetWaypoint = commuter.NextWaypoint;
            var waypointEntity = waypointsBuffer[targetWaypoint].Value;
            var targetPosition = GetComponent<Waypoint>(waypointEntity).WorldPosition; // TODO: DO NOT DO THIS! use entity query
            translation.Value = targetPosition;
            var waypointsCount = waypointsBuffer.Length;
            var nextWaypoint = targetWaypoint + 1;
            if (nextWaypoint < waypointsCount)
                commuter.NextWaypoint = nextWaypoint;
        }).ScheduleParallel();
    }
}
