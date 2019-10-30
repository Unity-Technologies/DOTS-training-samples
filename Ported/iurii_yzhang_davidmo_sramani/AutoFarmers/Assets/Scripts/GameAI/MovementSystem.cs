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
            Entities
                //.WithReadOnly(typeof(TargetTag))
                .ForEach((Entity entity, int entityInQueryIndex, ref AnimationCompleteTag _, ref TargetTag target,
                          ref Translation worldPosition) =>
                {
                    int2 tilePosition = RenderingUnity.World2TilePosition(worldPosition.Value);
                    if (tilePosition.x == target.TargetPosition.x &&
                        tilePosition.y == target.TargetPosition.y) {
                        ecb.RemoveComponent<TargetTag>(entityInQueryIndex, entity);
                    }
                }).Schedule(inputDeps);

            return inputDeps;
        }
    }
}