using Onboarding.BezierPath;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SplineFollowerSystem : SystemBase
{
    protected override void OnUpdate() {
        float time = (float)Time.ElapsedTime;
        float distance = time * 2.0f;

        Entities
            .WithAll<SplineFollower>()
            .ForEach((ref Translation translation,
                      ref Rotation rotation,
                      ref Spline spline) =>
            {
                ref var splineData = ref spline.splinePath.Value;
                
                int dummySegment = 0;
                SplineInterpolationHelper.InterpolatePosition(ref splineData, ref dummySegment, distance, out translation.Value);
            }).ScheduleParallel();
    }
}
