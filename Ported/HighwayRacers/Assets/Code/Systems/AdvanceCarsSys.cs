using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(SetTransformAndColorSys))]
    [AlwaysUpdateSystem]
    public class AdvanceCarsSys : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            var segmentLengths = RoadSys.segmentLengths;
            var buckets = RoadSys.CarBuckets;
            buckets.AdvanceCars(segmentLengths);
        }
    }
}