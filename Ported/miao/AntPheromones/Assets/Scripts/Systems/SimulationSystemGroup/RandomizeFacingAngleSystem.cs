using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace AntPheromones_ECS
{
    public class RandomizeFacingAngleSystem : JobComponentSystem
    {
        private EntityQuery _steeringStrengthQuery;
        private (bool IsRetrieved, float Value) _randomSteeringStrength;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!this._randomSteeringStrength.IsRetrieved)
            {
                this._randomSteeringStrength = 
                    (IsRetrieved: true,
                    Value: GetEntityQuery(ComponentType.ReadOnly<SteeringStrength>())
                               .GetSingleton<SteeringStrength>().Random);
            }
            return new Job
            {
                RandomSeed = (uint)Time.frameCount + 1,
                RandomSteeringStrength = this._randomSteeringStrength.Value
            }.Schedule(this, inputDeps);
        }
        
        [BurstCompile]
        private struct Job : IJobForEachWithEntity<FacingAngle>
        {
            public uint RandomSeed;
            public float RandomSteeringStrength;

            private const int AwesomePalindromicPrimeNumber = 1300031;
            
            public void Execute(Entity entity, int index, [WriteOnly] ref FacingAngle facingAngle)
            {
                facingAngle.Value += 
                    new Random(seed: (uint)(index + 1) * this.RandomSeed * AwesomePalindromicPrimeNumber)
                        .NextFloat(-this.RandomSteeringStrength, this.RandomSteeringStrength);
            }
        }
    }
}