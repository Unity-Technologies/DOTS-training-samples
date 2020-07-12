using Unity.Entities;

namespace HighwayRacer
{
    [UpdateAfter(typeof(CameraSys))]
    [AlwaysUpdateSystem]
    public class MergeInProgressSys : SystemBase 
    {
        protected override void OnUpdate()
        {
            var buckets = RoadSys.CarBuckets;
            var dt = Time.DeltaTime;
            buckets.Merge(dt);
        }
    }
}