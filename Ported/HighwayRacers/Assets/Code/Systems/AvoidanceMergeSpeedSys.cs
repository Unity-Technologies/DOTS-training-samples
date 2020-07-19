using Unity.Entities;

namespace HighwayRacer
{
    [UpdateAfter(typeof(CameraSys))]
    [AlwaysUpdateSystem]
    public class AvoidanceMergeSpeedSys : SystemBase
    {
        private bool mergeLeftFrame = false;
        
        protected override void OnUpdate()
        {
            mergeLeftFrame = !mergeLeftFrame;
            var buckets = RoadSys.CarBuckets;
            var dt = Time.DeltaTime;
            var jobHandle = buckets.UpdateCarsJob(dt, mergeLeftFrame, Dependency);
            jobHandle.Complete();
        }
    }
}