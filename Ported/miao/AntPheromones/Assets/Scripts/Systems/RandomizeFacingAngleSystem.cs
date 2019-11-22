using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
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
                    Value: GetEntityQuery(ComponentType.ReadOnly<SteeringStrengthComponent>())
                               .GetSingleton<SteeringStrengthComponent>().Random);
            }
            return new Job
            {
                RandomSeed = (uint)Time.frameCount + 1,
                RandomSteeringStrength = this._randomSteeringStrength.Value
            }.Schedule(this, inputDeps);
        }
        
        [BurstCompile]
        private struct Job : IJobForEachWithEntity<FacingAngleComponent>
        {
            public uint RandomSeed;
            public float RandomSteeringStrength;
            
            public void Execute(Entity e, int idx, [WriteOnly] ref FacingAngleComponent facingAngleComponent)
            {
                facingAngleComponent.Value += 
                    new Random(seed: (uint)(idx + 1) * this.RandomSeed * 1300031).NextFloat(-this.RandomSteeringStrength, this.RandomSteeringStrength);
            }
        }
    }
}