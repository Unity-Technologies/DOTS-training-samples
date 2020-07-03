using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace HighwayRacer
{
    // update cars that aren't merging or overtaking 
    [UpdateAfter(typeof(MergingSpeedSys))]
    public class RegularCarAvoidanceSys : SystemBase
    {
        const int nLanes = Road.nLanes;
        const float minDist = Road.minDist;

        const float mergeLookAhead = Road.mergeLookAhead;
        const float mergeLookBehind = Road.mergeLookBehind;

        const float decelerationRate = Road.decelerationRate;
        const float accelerationRate = Road.accelerationRate;

        private EntityCommandBufferSystem beginSim = new BeginSimulationEntityCommandBufferSystem();

        protected override void OnCreate()
        {
            base.OnCreate();
            beginSim = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            var nSegments = Road.nSegments;
            var trackLength = Road.roadLength;
            var roadSegments = Road.roadSegments;
            var segmentizedCars = World.GetExistingSystem<BucketizeSys>().BucketizedCars;
            var mergeLeftFrame = SegmentizeSys.mergeLeftFrame;
            var dt = Time.DeltaTime;

            // make sure we don't hit next car ahead, and trigger overtake state
            var ecb = beginSim.CreateCommandBuffer().ToConcurrent();

            var jobHandle = Entities.WithName("CarSys").WithNone<MergingLeft, MergingRight>().WithNone<OvertakingLeft, OvertakingRight>().ForEach((Entity ent,
                int entityInQueryIndex, ref TargetSpeed targetSpeed,
                ref Speed speed, ref Lane lane, in TrackPos trackPos, in TrackSegment trackSegment, in Blocking blocking, in DesiredSpeed desiredSpeed) =>
            {
                CarUtil.GetClosestPosAndSpeed(out var closestPos, out var closestSpeed,
                    segmentizedCars, trackSegment.Val, lane.Val,
                    trackLength, trackPos, nSegments);

                if (closestPos != float.MaxValue)
                {
                    var dist = closestPos - trackPos.Val;
                    if (dist <= blocking.Dist &&
                        speed.Val > closestSpeed) // car is still blocked ahead in lane
                    {
                        var closeness = (dist - minDist) / (blocking.Dist - minDist); // 0 is max closeness, 1 is min

                        // closer we get within minDist of leading car, the closer we match speed
                        const float fudge = 2.0f;
                        var newSpeed = math.lerp(closestSpeed, speed.Val + fudge, closeness);
                        if (newSpeed < speed.Val)
                        {
                            speed.Val = newSpeed;
                        }

                        // to spare us from having to check prior segment, can't merge if too close to start of segment
                        float segmentPos = (trackSegment.Val == 0) ? trackPos.Val : trackPos.Val - roadSegments[trackSegment.Val - 1].Threshold;
                        if (segmentPos < mergeLookBehind)
                        {
                            return;
                        }

                        // look for opening on left
                        if (mergeLeftFrame && lane.Val < nLanes - 1)
                        {
                            var leftLaneIdx = lane.Val + 1;
                            if (CarUtil.canMerge(trackPos.Val, leftLaneIdx, trackSegment.Val, segmentizedCars, trackLength, nSegments))
                            {
                                ecb.AddComponent<MergingLeft>(entityInQueryIndex, ent);
                                ecb.AddComponent<LaneOffset>(entityInQueryIndex, ent, new LaneOffset() {Val = -1.0f});
                                ecb.SetComponent<Lane>(entityInQueryIndex, ent, new Lane() {Val = (byte) leftLaneIdx});
                            }
                        }
                        else if (!mergeLeftFrame && lane.Val > 0) // look for opening on right
                        {
                            var rightLaneIdx = lane.Val - 1;
                            if (CarUtil.canMerge(trackPos.Val, rightLaneIdx, trackSegment.Val, segmentizedCars, trackLength, nSegments))
                            {
                                ecb.AddComponent<MergingRight>(entityInQueryIndex, ent);
                                ecb.AddComponent<LaneOffset>(entityInQueryIndex, ent, new LaneOffset() {Val = 1.0f});
                                ecb.SetComponent<Lane>(entityInQueryIndex, ent, new Lane() {Val = (byte) rightLaneIdx});
                            }
                        }

                        return;
                    }
                }

                CarUtil.SetUnblockedSpeed(ref speed, ref targetSpeed, dt, desiredSpeed.Unblocked);
            }).ScheduleParallel(Dependency);

            beginSim.AddJobHandleForProducer(jobHandle);
            Dependency = jobHandle;
        }
    }
}


static class BringYourOwnDelegate
{
    // Declare the delegate that takes 12 parameters. T0 is used for the Entity argument
    [Unity.Entities.CodeGeneratedJobForEach.EntitiesForEachCompatible]
    public delegate void CustomForEachDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8>
    (T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, in T5 t5,
        in T6 t6, in T7 t7, in T8 t8);

    // Declare the function overload
    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7, T8>
        (this TDescription description, CustomForEachDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8> codeToRun)
        where TDescription : struct, Unity.Entities.CodeGeneratedJobForEach.ISupportForEachWithUniversalDelegate =>
        LambdaForEachDescriptionConstructionMethods.ThrowCodeGenException<TDescription>();
}