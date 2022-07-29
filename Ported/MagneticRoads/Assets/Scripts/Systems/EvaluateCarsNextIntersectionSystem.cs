using Aspects;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    // [UpdateAfter(typeof(CarEvaluateSplinePositionSystem))]
    [BurstCompile]
    public partial struct EvaluateCarsNextIntersectionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) { }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var carAspect in SystemAPI.Query<CarAspect>().WithAll<WaitingAtIntersection>())
            {
                var nextIntersection = carAspect.NextIntersection;
                
                foreach (var roadSegment in SystemAPI.Query<RoadSegmentAspect>())
                {
                    // Debug.Log(carAspect.NextIntersection);
                    if (roadSegment.StartIntersection == carAspect.NextIntersection)
                    {
                        nextIntersection = roadSegment.EndIntersection;
                    }
                    else if (roadSegment.EndIntersection == carAspect.NextIntersection)
                    {
                        nextIntersection = roadSegment.StartIntersection;
                    }
                }

                carAspect.NextIntersection = nextIntersection;

            }
        }
    }
}
