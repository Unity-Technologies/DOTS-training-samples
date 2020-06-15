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
            RoadInit road = GameObject.FindObjectOfType<RoadInit>();
            if (road != null)
            {
                var segs = road.roadInfos;

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
                    segment.Val = (byte)(segs.Length - 1);  // last segment gets all the rest (better accounts for float imprecision)
                }).Run();
            }
        }
    }
}