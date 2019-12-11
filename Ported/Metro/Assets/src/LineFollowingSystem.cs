using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace src
{
    public class LineFollowingSystem : JobComponentSystem
    {
        const float k_SpeedIncrement = 0.001f;
        const float k_SpeedIncrementEpsilon = 0.0005f;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;
            
            
            return Entities
                .ForEach((ref Speed speed, in Translation translation) =>
                {
                    var distanceTravled = math.length(translation.Value - speed.LastPosition);
                    speed.CurrentSpeed = distanceTravled / deltaTime;
                    if(speed.CurrentSpeed + k_SpeedIncrementEpsilon < speed.TargetSpeed)
                    {
                        speed.Value += k_SpeedIncrement;
                    }
                    else if(speed.CurrentSpeed - k_SpeedIncrementEpsilon > speed.TargetSpeed)
                    {
                        Debug.Log("Decrease Speed");
                        speed.Value -= k_SpeedIncrement;
                        if (speed.Value < 0)
                            speed.Value = 0;
                    }
                    speed.LastPosition = translation.Value;
                })
                .Schedule(inputDeps);
        }
    }
}