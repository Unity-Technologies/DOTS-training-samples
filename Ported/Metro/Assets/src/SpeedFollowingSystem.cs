using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace src
{
    public class SpeedFollowingSystem : JobComponentSystem
    {
        const float k_SpeedIncrement = 0.001f;
        const float k_SpeedIncrementEpsilon = 0.0005f;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;
            
            
            return Entities
                .ForEach((ref Speed speed, ref TargetSpeed targetSpeed, in Translation translation) =>
                {
                    var distanceTravled = math.length(translation.Value - targetSpeed.LastPosition);
                    var currentSpeed = distanceTravled / deltaTime;
                    if(currentSpeed + k_SpeedIncrementEpsilon < targetSpeed.Value)
                    {
                        speed.Value += k_SpeedIncrement;
                    }
                    else if(currentSpeed- k_SpeedIncrementEpsilon > targetSpeed.Value)
                    {
                        speed.Value -= k_SpeedIncrement;
                        if (speed.Value < 0)
                            speed.Value = 0;
                    }
                    targetSpeed.LastPosition = translation.Value;
                })
                .Schedule(inputDeps);
        }
    }
}