using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class AdjustSpeedSystem : JobComponentSystem
    {
        struct Job : IJobForEach<Position, Speed, PheromoneSteering, WallSteering>
        {
            public void Execute(
                [ReadOnly] ref Position position, 
                [WriteOnly] ref Speed speed,
                [ReadOnly] ref PheromoneSteering pheromoneSteering, 
                [ReadOnly] ref WallSteering wallSteering)
            {
                const float Acceleration = 0.07f;
                
                float targetSpeed = speed.Value * 1f - (Mathf.Abs(pheromoneSteering.Value) + Math.Abs(wallSteering.Value)) / 3f;
                speed.Value += (targetSpeed - speed.Value) * Acceleration;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job().Schedule(this, inputDeps);
        }
    }
}