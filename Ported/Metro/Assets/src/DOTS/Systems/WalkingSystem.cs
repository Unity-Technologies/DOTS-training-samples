using System.Linq;
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
        const float m_D = 2f;

        protected override void OnCreate()
        {
            CommandBufferSystem
                = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = CommandBufferSystem.CreateCommandBuffer();

            Entities.WithoutBurst().WithAll<WalkingTag>().ForEach((
                ref Translation translation,
                ref DynamicBuffer<PathData> pathData,
                in Entity commuter) =>
            {
                int lastIndex = pathData.Length - 1;

                //if (lastIndex < 0)
                  //  return;
                
                PathData currentNavPoint = pathData[lastIndex];
                
                if (math.distancesq(translation.Value, currentNavPoint.point) < m_D * deltaTime * m_D * deltaTime)
                {
                    translation.Value = currentNavPoint.point;
                    
                    pathData.RemoveAt(lastIndex);

                    if (lastIndex == 0)
                    {
                        ecb.RemoveComponent<WalkingTag>(commuter);
                    }

                    return;
                }
                
                //TODO: pop the last NavPoint instead of checking against max length
                
                var distance = currentNavPoint.point - translation.Value;
                var direction = math.normalize(distance);
                translation.Value += direction * m_D * deltaTime;
            }).Run();
        }
    }
}