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
            var deltaTime = Time.DeltaTime;
            var numModels = m_NumModels;
            var rng = new Random((1 + (uint)UnityEngine.Time.frameCount) * 104729);
            inputDeps.Complete();
            Entities.ForEach((ref DistanceFieldComponent distanceField) =>
            {
                distanceField.TimeToSwitch -= deltaTime;
                if (distanceField.TimeToSwitch <= 0)
                {
                    distanceField.TimeToSwitch = 2;
                    distanceField.ModelType = (DistanceFieldModel) rng.NextInt(numModels);
                }
            }).Run();
            return default(JobHandle);
        }
    }
}
