using src.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace src.DOTS.Systems
{
    public class WalkingSystem : SystemBase
    {
        private EntityCommandBufferSystem CommandBufferSystem;
        const float m_D = 1f;

        protected override void OnCreate()
        {
            CommandBufferSystem
                = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = CommandBufferSystem.CreateCommandBuffer();

            Entities.WithoutBurst().WithAll<SwitchingPlatformTag>().ForEach((
                ref Translation translation,
                ref CurrentPathTarget currentPathTarget,
                in DynamicBuffer<PathData> pathData,
                in Entity commuter) =>
            {
                if (math.distancesq(translation.Value, pathData[currentPathTarget.currentIndex].point) < m_D * deltaTime * m_D * deltaTime)
                {
                    translation.Value = pathData[currentPathTarget.currentIndex].point;
                    currentPathTarget.currentIndex++;
                    if (currentPathTarget.currentIndex > 3)
                    {
                        ecb.RemoveComponent<SwitchingPlatformTag>(commuter);
                    }
                    return;
                }
                var distance = pathData[currentPathTarget.currentIndex].point - translation.Value;
                var direction = math.normalize(distance);
                translation.Value += direction * m_D * deltaTime;
            }).Run();
        }
    }
}