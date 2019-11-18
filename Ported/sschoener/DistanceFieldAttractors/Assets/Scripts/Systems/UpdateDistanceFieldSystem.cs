using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class UpdateDistanceFieldSystem : JobComponentSystem
    {
        readonly int m_NumModels = Enum.GetValues(typeof(DistanceFieldModel)).Length;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // this is very much overkill, since scheduling the job is likely much more expensive than just
            // updating on the main thread.
            return new UpdateJob
            {
                DeltaTime = Time.deltaTime,
                NumModels = m_NumModels,
                Rng = new Random((1 + (uint)Time.frameCount) * 104729)
            }.Run(this, inputDeps);
        }

        [BurstCompile]
        struct UpdateJob : IJobForEach<DistanceFieldComponent>
        {
            public float DeltaTime;
            public int NumModels;
            public Random Rng;

            public void Execute(ref DistanceFieldComponent distanceField)
            {
                distanceField.TimeToSwitch -= DeltaTime;
                if (distanceField.TimeToSwitch <= 0)
                {
                    distanceField.TimeToSwitch = 2;
                    distanceField.ModelType = (DistanceFieldModel) Rng.NextInt(NumModels);
                }
            }
        }
    }
}
