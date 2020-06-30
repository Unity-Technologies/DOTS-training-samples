using Unity.Entities;
using Unity.Jobs;
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
            if (Road.roadSegments.IsCreated)
            {
                float roadLength = Road.roadLength;

                Entity e = EntityManager.CreateEntity();

                var dt = Time.DeltaTime;

                Entities.ForEach((ref TrackPos trackPos, in Speed speed) =>
                {
                    trackPos.Val += speed.Val * dt;
                    if (trackPos.Val > roadLength)
                    {
                        trackPos.Val -= roadLength;
                    }
                }).ScheduleParallel();
            }
        }
    }
}