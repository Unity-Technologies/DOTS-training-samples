using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateBefore(typeof(AdvanceCarsSys))]
    [UpdateAfter(typeof(MergingSpeedSys))]
    public class OvertakingSys : SystemBase
    {
        private NativeArray<OtherCars> selection; // the OtherCar segments to compare against a particular car
    
        const int nSegments = RoadInit.nSegments;
    
        const float minDist = RoadInit.minDist;
    
        const float mergeLookBehind = RoadInit.mergeLookBehind;
    
        const float decelerationRate = RoadInit.decelerationRate;
        const float accelerationRate = RoadInit.accelerationRate;
    
        const float overtakeTimeBeforeMerge = 3.0f;
        const float overtakeTimeout = overtakeTimeBeforeMerge + 3.0f;
    
        protected override void OnCreate()
        {
            base.OnCreate();
    
            selection = new NativeArray<OtherCars>(2, Allocator.Persistent);
        }
    
        protected override void OnDestroy()
        {
            base.OnDestroy();
            selection.Dispose();
        }
    
        protected override void OnUpdate()
        {
            var trackLength = RoadInit.trackLength;
            var roadSegments = RoadInit.roadSegments;
    
            var selection = this.selection;
            var otherCars = World.GetExistingSystem<CarsByLaneSegmentSys>().otherCars;
            
            var mergeLeftFrame = SegmentizeSys.mergeLeftFrame;
    
            var dt = Time.DeltaTime;
            var time = Time.ElapsedTime;
            
            var leftECB = new EntityCommandBuffer(Allocator.TempJob);
            var rightECB = new EntityCommandBuffer(Allocator.TempJob);

            Entities.WithNone<MergingLeft>().ForEach((Entity ent, ref Speed speed, ref Lane lane, in DesiredSpeed desiredSpeed,
                in OvertakingLeft overtakingLeft, in TrackPos trackPos, in TrackSegment trackSegment, in Blocking blockingInfo) =>
            {
                var laneBaseIdx = lane.Val * nSegments;
    
                var idx = laneBaseIdx + trackSegment.Val;
                selection[0] = otherCars[idx];
    
                // next
                idx = laneBaseIdx + ((trackSegment.Val == nSegments - 1) ? 0 : trackSegment.Val + 1);
                selection[1] = otherCars[idx];
    
                CarSys.GetClosestPosAndSpeed(out var closestPos, out var closestSpeed, selection, trackSegment, trackLength, trackPos);
                
                // if blocked, leave OvertakingLeft state
                if (closestPos != float.MaxValue)
                {
                    var dist = closestPos - trackPos.Val;
                    if (dist <= blockingInfo.Dist &&
                        speed.Val > closestSpeed) // car is blocked ahead in lane
                    {
                        var closeness = (dist - minDist) / (blockingInfo.Dist - minDist); // 0 is max closeness, 1 is min
    
                        // closer we get within minDist of leading car, the closer we match speed
                        const float fudge = 2.0f;
                        var newSpeed = math.lerp(closestSpeed, speed.Val + fudge, closeness);
                        if (newSpeed < speed.Val)
                        {
                            speed.Val = newSpeed;
                        }
                        
                        leftECB.RemoveComponent<OvertakingLeft>(ent);
                        leftECB.SetComponent(ent, new TargetSpeed() { Val = desiredSpeed.Unblocked} );
                        return;
                    }
                }
                
                var elapsedSinceOvertake = time - overtakingLeft.Time;

                // merging timed out, so end overtake
                if (elapsedSinceOvertake > overtakeTimeout)
                {
                    leftECB.RemoveComponent<OvertakingLeft>(ent);
                    leftECB.SetComponent(ent, new TargetSpeed() { Val = desiredSpeed.Unblocked} );
                    return;
                }
    
                // to spare us from having to check prior segment, can't merge if too close to start of segment
                float segmentPos = (trackSegment.Val == 0) ? trackPos.Val : trackPos.Val - roadSegments[trackSegment.Val - 1].Threshold;
                if (segmentPos < mergeLookBehind)
                {
                    return;
                }
                        
                // if enough time elapsed since starting overtake, look for opening on left
                if (elapsedSinceOvertake > overtakeTimeBeforeMerge && mergeLeftFrame)
                {
                    var leftLaneIdx = lane.Val + 1;
                    if (CarSys.canMerge(trackPos.Val, leftLaneIdx, trackSegment.Val, otherCars, trackLength))
                    {
                        leftECB.AddComponent<MergingLeft>(ent);
                        leftECB.AddComponent<LaneOffset>(ent, new LaneOffset() {Val = -1.0f});
                        lane.Val = (byte) leftLaneIdx;
                        return;
                    }
                }
    
                if (desiredSpeed.Overtake < speed.Val)
                {
                    speed.Val -= decelerationRate * dt;
                    if (speed.Val < desiredSpeed.Overtake)
                    {
                        speed.Val = desiredSpeed.Overtake;
                    }
                }
                else if (desiredSpeed.Overtake > speed.Val)
                {
                    speed.Val += accelerationRate * dt;
                    if (speed.Val > desiredSpeed.Overtake)
                    {
                        speed.Val = desiredSpeed.Overtake;
                    }
                }
            }).Run();

            Entities.WithNone<MergingRight>().ForEach((Entity ent, ref Speed speed, ref Lane lane, in DesiredSpeed desiredSpeed,
                in OvertakingRight overtakingRight, in TrackPos trackPos, in TrackSegment trackSegment, in Blocking blockingInfo) =>
            {
                var laneBaseIdx = lane.Val * nSegments;
    
                var idx = laneBaseIdx + trackSegment.Val;
                selection[0] = otherCars[idx];
    
                // next
                idx = laneBaseIdx + ((trackSegment.Val == nSegments - 1) ? 0 : trackSegment.Val + 1);
                selection[1] = otherCars[idx];
    
                CarSys.GetClosestPosAndSpeed(out var closestPos, out var closestSpeed, selection, trackSegment, trackLength, trackPos);
                
                // if blocked, leave OvertakingRight state
                if (closestPos != float.MaxValue)
                {
                    var dist = closestPos - trackPos.Val;
                    if (dist <= blockingInfo.Dist &&
                        speed.Val > closestSpeed) // car is blocked ahead in lane
                    {
                        var closeness = (dist - minDist) / (blockingInfo.Dist - minDist); // 0 is max closeness, 1 is min
    
                        // closer we get within minDist of leading car, the closer we match speed
                        const float fudge = 2.0f;
                        var newSpeed = math.lerp(closestSpeed, speed.Val + fudge, closeness);
                        if (newSpeed < speed.Val)
                        {
                            speed.Val = newSpeed;
                        }
                        
                        rightECB.RemoveComponent<OvertakingRight>(ent);
                        rightECB.SetComponent(ent, new TargetSpeed() { Val = desiredSpeed.Unblocked} );
                        return;
                    }
                }
                
                var elapsedSinceOvertake = time - overtakingRight.Time;
                        
                // merging timed out, so end overtake
                if (elapsedSinceOvertake > overtakeTimeout)
                {
                    rightECB.RemoveComponent<OvertakingRight>(ent);
                    rightECB.SetComponent(ent, new TargetSpeed() { Val = desiredSpeed.Unblocked} );
                    return;
                }
    
                // to spare us from having to check prior segment, can't merge if too close to start of segment
                var threshold = roadSegments[(trackSegment.Val > 0) ? trackSegment.Val - 1 : nSegments - 1].Threshold;
                var segmentPos = trackPos.Val - threshold;
                if (segmentPos < mergeLookBehind)
                {
                    return;
                }
                        
                // if enough time elapsed since starting overtake, look for opening on right
                if (elapsedSinceOvertake > overtakeTimeBeforeMerge && !mergeLeftFrame)
                {
                    var rightLaneIdx = lane.Val - 1;
                    if (CarSys.canMerge(trackPos.Val, rightLaneIdx, trackSegment.Val, otherCars, trackLength))
                    {
                        rightECB.AddComponent<MergingRight>(ent);
                        rightECB.AddComponent<LaneOffset>(ent, new LaneOffset() {Val = +1.0f});
                        lane.Val = (byte) rightLaneIdx;
                        return;
                    }
                }
                
                if (desiredSpeed.Overtake < speed.Val)
                {
                    speed.Val -= decelerationRate * dt;
                    if (speed.Val < desiredSpeed.Overtake)
                    {
                        speed.Val = desiredSpeed.Overtake;
                    }
                }
                else if (desiredSpeed.Overtake > speed.Val)
                {
                    speed.Val += accelerationRate * dt;
                    if (speed.Val > desiredSpeed.Overtake)
                    {
                        speed.Val = desiredSpeed.Overtake;
                    }
                }
            }).Run();
    
            leftECB.Playback(EntityManager);
            leftECB.Dispose();
            
            rightECB.Playback(EntityManager);
            rightECB.Dispose();
        }
    }
}