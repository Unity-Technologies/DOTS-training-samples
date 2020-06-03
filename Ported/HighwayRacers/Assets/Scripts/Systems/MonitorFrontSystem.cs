using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MonitorFrontSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //setup all other native arrays
        NativeArray<TrackPosition> otherCars = GetEntityQuery(typeof(TrackPosition), typeof(Speed)).ToComponentDataArray<TrackPosition>(Allocator.TempJob);
        NativeArray<Speed> otherSpeeds = GetEntityQuery(typeof(TrackPosition), typeof(Speed)).ToComponentDataArray<Speed>(Allocator.TempJob);

        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref CarInFront carInFront, in TrackPosition car) =>
            {
                // 1. get current progress
                float progress = car.TrackProgress;
                float firstInLaneProgress = float.MaxValue;
                float firstInLaneSpeed = 0;
                float inFrontProgress = float.MaxValue;
                float inFrontSpeed = 0;
                int lane = (int) car.Lane;
                Entity e = entity;

                // 2. sort through others and find closest (in front) in the same lane
                for (int i = 0; i < otherCars.Length; i++)
                {
                    if ((int) otherCars[i].Lane == lane)
                    {
                        if (otherCars[i].TrackProgress > progress && otherCars[i].TrackProgress < inFrontProgress)
                        {
                            inFrontProgress = otherCars[i].TrackProgress;
                            inFrontSpeed = otherSpeeds[i].Value;
                        }

                        if (otherCars[i].TrackProgress < firstInLaneProgress)
                        {
                            firstInLaneProgress = otherCars[i].TrackProgress;
                            firstInLaneSpeed = otherSpeeds[i].Value;
                        }
                    }
                }

                // if there's no car in front assigned, use the first car in track
                if (inFrontSpeed == 0)
                {
                    inFrontProgress = firstInLaneProgress;
                    inFrontSpeed = firstInLaneSpeed;
                }

                // 3. set the values for the car in front
                // write the data back to CarInFront component
                carInFront.TrackProgressCarInFront = inFrontProgress;
                carInFront.Speed = inFrontSpeed;
            })
            .WithDeallocateOnJobCompletion(otherCars)
            .WithDeallocateOnJobCompletion(otherSpeeds)
            .ScheduleParallel();
    }
}
