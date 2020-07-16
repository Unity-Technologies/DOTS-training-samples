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
        var deltaTime = Time.DeltaTime;
        Entities
            .WithAll<CommuterWalking>()
            .ForEach((ref Commuter commuter, ref Translation translation, in CommuterSpeed commuterSpeed, in DynamicBuffer<CommuterWaypoint> waypointsBuffer) =>
            {
                var targetWaypoint = commuter.NextWaypoint;
                var waypointEntity = waypointsBuffer[targetWaypoint].Value;
                var targetPosition = GetComponent<Waypoint>(waypointEntity).WorldPosition; // TODO: DO NOT DO THIS! use entity query
                var distanceToMove = commuterSpeed.Value * deltaTime;
                var currentPosition = translation.Value;
                if (math.distancesq(currentPosition, targetPosition) < distanceToMove * distanceToMove)
                {
                    translation.Value = targetPosition;
                    var waypointsCount = waypointsBuffer.Length;
                    var nextWaypoint = targetWaypoint + 1;
                    if (nextWaypoint < waypointsCount)
                    {
                        commuter.NextWaypoint = nextWaypoint;
                        var nextWaypointEntity = waypointsBuffer[nextWaypoint].Value;
                        var nextWaypointPosition = GetComponent<Waypoint>(nextWaypointEntity).WorldPosition;
                        commuter.Direction = math.normalize(nextWaypointPosition - translation.Value);
                    }
                }
                else
                {
                    translation.Value = currentPosition + commuter.Direction * distanceToMove;
                }
            }).ScheduleParallel();
    }
}
