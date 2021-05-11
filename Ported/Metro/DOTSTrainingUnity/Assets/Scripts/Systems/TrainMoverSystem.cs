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

        Entities.ForEach((
            ref TrainCurrDistance currDist, ref TrainCurrSpeed currSpeed,
            in TrainTargetDistance targetDist, in TrainTotalDistance totalDist,
            in TrainMaxSpeed maxSpeed) => { 
                currSpeed.value = maxSpeed.value;
                float distToTarget = targetDist.value - currDist.value;
                if (distToTarget < 0f)
                {
                    distToTarget = totalDist.value - currDist.value + targetDist.value;
                }

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
                }
            }).Schedule();
    }
}
