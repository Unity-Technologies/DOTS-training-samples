using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainMoverSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float stopTolerance = 0.05f;
        float minSpeed = 1f;
        float acceleration = 1.2f;
        Entities.ForEach((
            ref TrainState trainState,
            ref TrainCurrDistance currDist, ref TrainCurrSpeed currSpeed,
            in TrainTargetDistance targetDist, in TrainTotalDistance totalDist,
            in TrainMaxSpeed maxSpeed) => 
        {
            float stoppingTimeSecs = currSpeed.value / acceleration;
            float stoppingDistance = 0.5f * acceleration * stoppingTimeSecs * stoppingTimeSecs + currSpeed.value * stoppingTimeSecs;

            // Calculate distance to target taking into account the fact that we can loop
            float distToTarget = targetDist.value - currDist.value;
            if (distToTarget < 0f)
            {
                distToTarget = totalDist.value - currDist.value + targetDist.value;
            }

            // If we are not at max speed and do not need to slow down
            if (currSpeed.value < maxSpeed.value && distToTarget > stoppingDistance)
            {
                currSpeed.value = math.mad(acceleration, deltaTime, currSpeed.value);
            }
            // If we are in stopping distance (any later and we'll miss the target)
            else if (distToTarget <= stoppingDistance && distToTarget > stopTolerance)
            {
                currSpeed.value = math.max(currSpeed.value - acceleration * deltaTime, minSpeed);
            }
            else if (distToTarget <= stopTolerance)
            {
                currSpeed.value = 0f;
            }

            // Update train distance based on current speed
            if (distToTarget >= currSpeed.value * deltaTime)
            {
                currDist.value += currSpeed.value * deltaTime;
                if (currDist.value > totalDist.value)
                {
                    currDist.value -= totalDist.value;
                }
            } 
            else
            {
                currDist.value = targetDist.value;
                if (trainState.value == CurrTrainState.Moving)
                {
                    trainState.value = CurrTrainState.Waiting;
                }
            }
        }).Schedule();
    }
}
