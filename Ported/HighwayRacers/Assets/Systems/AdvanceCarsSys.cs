using Unity.Entities;
using UnityEngine;

namespace HighwayRacer
{
    public class AdvanceCarsSys : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (RoadInit.roadSegments.IsCreated)
            {
                float trackLength = RoadInit.roadSegments[RoadInit.roadSegments.Length - 1].Threshold;

                var dt = Time.DeltaTime;
            
                Entities.ForEach((ref TrackPos trackPos, in Speed speed) =>
                {
                    trackPos.Val += speed.Val * dt;
                    if (trackPos.Val > trackLength)
                    {
                        trackPos.Val -= trackLength;
                    }
                }).Run();
            }
        }
    }
}