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
                //.WithReadOnly(typeof(HasTarget))
                .ForEach((Entity entity, int entityInQueryIndex, ref AnimationCompleteTag _, ref HasTarget target, ref Translation worldPosition) =>
                {
                    int2 tilePosition = RenderingUnity.World2TilePosition(worldPosition.Value);
                    if (tilePosition.x == target.TargetPosition.x &&
                        tilePosition.y == target.TargetPosition.y) {
                        ecb.RemoveComponent<HasTarget>(entityInQueryIndex, entity);
                    } else {
                        var direction =  math.normalize(target.TargetPosition - tilePosition);
                        worldPosition.Value += new float3(direction.x,0,direction.y);
                    }
                }).Schedule(inputDeps);

            return inputDeps;
        }
    }
}