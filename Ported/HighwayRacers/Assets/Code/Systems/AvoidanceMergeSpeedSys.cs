using Unity.Entities;

namespace HighwayRacer
{
    [UpdateAfter(typeof(CameraSys))]
    [AlwaysUpdateSystem]
    public class AvoidanceMergeSpeedSys : SystemBase 
    {
        protected override void OnUpdate()
        {
            var buckets = RoadSys.CarBuckets;
            var dt = Time.DeltaTime;
            //buckets.UpdateCars(dt);
        }
    }
}