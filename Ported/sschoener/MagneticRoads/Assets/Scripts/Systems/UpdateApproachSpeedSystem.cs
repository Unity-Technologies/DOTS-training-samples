using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateSpeedSystem))]
    public class UpdateApproachSpeedSystem : JobComponentSystem
    {
        RoadQueueSystem m_RoadQueueSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_RoadQueueSystem = World.GetExistingSystem<RoadQueueSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var blobRef = GetSingleton<RoadSetupComponent>().Splines;
            var occupation = m_RoadQueueSystem.IntersectionOccupation;

            var queues = m_RoadQueueSystem.Queues;
            var queueEntries = m_RoadQueueSystem.QueueEntries;
            return Entities.ForEach((Entity entity, ref CarSpeedComponent speed, in OnSplineComponent onSpline, in VehicleStateComponent vehicleState) =>
            {
                float approachSpeed = 1f;
                if (vehicleState != VehicleState.OnRoad)
                    approachSpeed = .7f;
                else
                {
                    int queueIndex = RoadQueueSystem.GetQueueIndex(onSpline.Value);
                    var queue = queues[queueIndex];

                    if (queueEntries[queue.Begin].Entity != entity)
                    {
                        // someone's ahead of us - don't clip through them
                        int index = -1;
                        for (int i = queue.Begin; i < queue.End; i++)
                        {
                            if (queueEntries[i].Entity == entity)
                            {
                                index = i;
                                break;
                            }
                        }
                        float queueSize = blobRef.Value.Splines[onSpline.Value.Spline].CarQueueSize;
                        float maxT = queueEntries[index - 1].SplineTimer - queueSize;
                        speed.SplineTimer = math.min(speed.SplineTimer, maxT);
                        approachSpeed = (maxT - speed.SplineTimer) * 5f;
                    }
                    else
                    {
                        // we're "first in line" in our lane, but we still might need
                        // to slow down if our next intersection is occupied
                        var s = onSpline.Value.Spline;
                        ref var spl = ref blobRef.Value.Splines[s];
                        var target = onSpline.Value.Direction == 1 ? spl.EndIntersection : spl.StartIntersection;
                        if (occupation[target/4][(uint)(target % 4 + (onSpline.Value.Side + 1) / 2)])
                            approachSpeed = (1f - speed.SplineTimer) * .8f + .2f;
                    }
                }
                speed.NormalizedSpeed = math.min(speed.NormalizedSpeed, approachSpeed);
            }).WithoutBurst().WithName("UpdateApproachSpeed").Schedule(inputDeps);
        }
    }
}
