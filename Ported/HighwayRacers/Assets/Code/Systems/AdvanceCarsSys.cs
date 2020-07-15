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
            var dt = Time.DeltaTime;
            
            if (buckets.IsCreated)
            {
                buckets.AdvanceCarsJob(segmentLengths, dt, Dependency);
            }
        }
    }
}