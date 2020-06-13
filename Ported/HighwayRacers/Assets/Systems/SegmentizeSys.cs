using Unity.Assertions;
using Unity.Entities;
using UnityEngine;

namespace HighwayRacer
{
    public class SegmentizeSys : SystemBase
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
                var thresholds = road.segmentThresholds;

                Entities.ForEach((ref TrackSegment segment, in TrackPos pos) =>
                {
                    for (byte i = 0; i < thresholds.Length - 1; i++)
                    {
                        if (pos.Val < thresholds[i])
                        {
                            segment.Val = i;
                            return;
                        }
                    }
                    segment.Val = (byte)(thresholds.Length - 1);  // last segment gets all the rest (better accounts for float imprecision)
                }).Run();
            }
        }
    }
}