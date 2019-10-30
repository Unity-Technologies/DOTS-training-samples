using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace GameAI
{
    public struct AnimationCompleteTag : IComponentData {}
    
    public class MovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Entities.WithReadOnly(typeof(TargetTag))
                    .ForEach((ref AnimationCompleteTag _, ref TargetTag target, ref Translation position) => {
                                 
                             });

            return inputDeps;
        }
    }
}