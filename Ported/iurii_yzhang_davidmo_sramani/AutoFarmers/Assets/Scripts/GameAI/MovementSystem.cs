using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GameAI
{
    public struct AnimationCompleteTag : IComponentData {}
    
    public class MovementSystem : JobComponentSystem
    {
        BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job = Entities
                //.WithReadOnly(typeof(HasTarget))
                .WithAll<AnimationCompleteTag>()
                .ForEach((Entity entity, int entityInQueryIndex, ref HasTarget target, ref TilePositionable tilePositionable) =>
                {
                    tilePositionable.Position = target.TargetPosition;
                    ecb.RemoveComponent<HasTarget>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            return JobHandle.CombineDependencies(job, inputDeps);
        }
    }
}