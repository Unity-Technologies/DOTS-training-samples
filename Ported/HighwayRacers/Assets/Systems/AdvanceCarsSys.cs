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
            RoadInit road = GameObject.FindObjectOfType<RoadInit>();
            if (road != null)
            {
                float trackLength = road.roadInfos[road.roadInfos.Length - 1].Threshold;

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