using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CommuterWalkSystem : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var concurrentECB = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        var deltaTime = Time.DeltaTime;
        Entities
            .WithAll<CommuterWalking>()
            .ForEach((Entity commuterEntity, int entityInQueryIndex, ref Commuter commuter,
                ref Translation translation, in CommuterSpeed commuterSpeed, in DynamicBuffer<CommuterWaypoint> waypointsBuffer) =>
            {
                var waypointsCount = waypointsBuffer.Length;
                var targetWaypointIndex = commuter.NextWaypoint;
                var nextWaypointIndex = targetWaypointIndex + 1;
                var waypointEntity = waypointsBuffer[targetWaypointIndex].Value;
                var targetPosition = GetComponent<Waypoint>(waypointEntity).WorldPosition; // TODO: DO NOT DO THIS! use entity query
                var distanceToMove = commuterSpeed.Value * deltaTime;
                var currentPosition = translation.Value;
                if (math.distancesq(currentPosition, targetPosition) < distanceToMove * distanceToMove)
                {
                    translation.Value = targetPosition;
                    if (nextWaypointIndex >= waypointsCount)
                    {
                        // Reached destination
                        concurrentECB.DestroyEntity(entityInQueryIndex, commuterEntity);
                    }
                    else
                    {
                        var nextWaypointEntity = waypointsBuffer[nextWaypointIndex].Value;
                        if (HasComponent<Platform>(waypointEntity))
                        {
                            if (HasComponent<Platform>(nextWaypointEntity))
                            {
                                concurrentECB.RemoveComponent<CommuterWalking>(entityInQueryIndex, commuterEntity);
                                concurrentECB.AddComponent(entityInQueryIndex, commuterEntity, new CommuterBoarding { QueueIndex = -1 });
                                commuter.NextPlatform = nextWaypointEntity;
                            }

                            commuter.CurrentPlatform = waypointEntity;
                        }

                        commuter.NextWaypoint = nextWaypointIndex;
                        var nextWaypointPosition = GetComponent<Waypoint>(nextWaypointEntity).WorldPosition;
                        commuter.Direction = math.normalize(nextWaypointPosition - translation.Value);
                    }
                }
                else
                {
                    translation.Value = currentPosition + commuter.Direction * distanceToMove;
                }
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
