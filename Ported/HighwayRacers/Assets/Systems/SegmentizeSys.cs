using Unity.Assertions;
using Unity.Entities;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(AdvanceCarsSys))]
    public class SegmentizeSys : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (RoadInit.roadSegments.IsCreated)
            {
                var segs = RoadInit.roadSegments;

                Entities.ForEach((ref TrackSegment segment, in TrackPos pos) =>
                {
                    for (byte i = 0; i < segs.Length - 1; i++)
                    {
                        if (pos.Val < segs[i].Threshold)
                        {
                            segment.Val = i;
                            return;
                        }
                    }
                    segment.Val = (byte)(segs.Length - 1);  // last segment gets all the rest (to account for float imprecision)
                }).Run();
            }
        }
    }
}