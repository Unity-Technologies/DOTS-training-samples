using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(SetTransformSys))]
    public class AdvanceCarsSys : SystemBase
    {
        public static bool mergeLeftFrame = true; // toggles every frame: in a frame, we only initiate merges either left or right, not both
        
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (RoadSys.roadSegments.IsCreated)
            {
                mergeLeftFrame = !mergeLeftFrame;
                
                var dt = Time.DeltaTime;  // todo: reliance on dt means could have bugs at very low framerates 

                ushort lastSegment = (ushort) (RoadSys.nSegments - 1);
                var segmentLengths = RoadSys.segmentLengths;

                var jobAdvanceEntities = Entities.WithNativeDisableContainerSafetyRestriction(segmentLengths).ForEach(
                    (ref TrackPos trackPos, ref Segment segment, ref SegmentLength segmentLength, in Speed speed) =>
                    {
                        trackPos.Val += speed.Val * dt;
                        if (trackPos.Val > segmentLength.Val)
                        {
                            trackPos.Val -= segmentLength.Val;
                            segment.Val = (segment.Val == lastSegment) ? (ushort) 0 : (ushort) (segment.Val + 1);
                            segmentLength.Val = segmentLengths[segment.Val];
                        }
                    }).ScheduleParallel(Dependency);

                var buckets = RoadSys.CarBuckets;
                var jobAdvanceBuckets = buckets.AdvanceCars(segmentLengths, Dependency);
                var jobSort = buckets.Sort(jobAdvanceBuckets);

                Dependency = JobHandle.CombineDependencies(jobAdvanceEntities, jobSort);
            }
        }
    }
}