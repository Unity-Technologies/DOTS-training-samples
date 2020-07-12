using Unity.Entities;

namespace HighwayRacer
{
    [UpdateAfter(typeof(MergeInProgressSys))]
    [AlwaysUpdateSystem]
    public class AvoidanceAndSpeedSys : SystemBase 
    {
        public static bool mergeLeftFrame = true; // toggles every frame: in a frame, we only initiate merges either left or right, not both
        
        protected override void OnUpdate()
        {
            mergeLeftFrame = !mergeLeftFrame;
            
            var buckets = RoadSys.CarBuckets;
            var dt = Time.DeltaTime;
            buckets.Avoidance(dt);
        }
    }
}