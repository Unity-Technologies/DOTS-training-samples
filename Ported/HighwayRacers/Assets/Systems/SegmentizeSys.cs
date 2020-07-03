using Unity.Assertions;
using Unity.Collections;
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

        public static bool mergeLeftFrame = true;     // toggles every frame: in a frame, we only initiate merges either left or right, not both
        
        protected override void OnUpdate()
        {
            mergeLeftFrame = !mergeLeftFrame;
            
            if (Road.roadSegments.IsCreated)
            {
                var thresholds = Road.thresholds;
                var lastIdx = thresholds.Length - 1;

                Entities.ForEach((ref TrackSegment segment, in TrackPos pos) =>
                {
                    for (byte i = 0; i < lastIdx; i++)
                    {
                        if (pos.Val < thresholds[i])    // todo: binary search (if num thresholds is large, might make sense)
                        {
                            segment.Val = i;
                            return;
                        }
                    }
                    segment.Val = (byte)lastIdx;  // last segment gets all the rest (to account for float imprecision)
                }).ScheduleParallel();
            }
        }
    }
}