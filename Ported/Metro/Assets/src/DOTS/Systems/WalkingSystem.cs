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
            var metro = this.GetSingleton<GameObjectRefs>();
            
            var deltaTime = Time.DeltaTime;
            var ecb = CommandBufferSystem.CreateCommandBuffer();

            Entities.WithoutBurst().ForEach((
                ref Translation translation,
                ref CurrentPathTarget currentPathTarget,
                ref DynamicBuffer<PathData> pathData,
                in Entity commuter) =>
            {
                if (math.distancesq(translation.Value, pathData[currentPathTarget.currentIndex].point) < m_D * deltaTime * m_D * deltaTime)
                {
                    translation.Value = pathData[currentPathTarget.currentIndex].point;
                    currentPathTarget.currentIndex++;
                    
                    // TODO: move to switching platform system
                    if (currentPathTarget.currentIndex > pathData.Length)
                    {
                        //ecb.RemoveComponent<SwitchingPlatformTag>(commuter);
                        
                        ecb.RemoveComponent<CurrentPathTarget>(commuter);

                        pathData.Clear();
                    }
                    return;
                }
                
                //TODO: pop the last NavPoint instead of checking against max length
                
                var distance = pathData[currentPathTarget.currentIndex].point - translation.Value;
                var direction = math.normalize(distance);
                translation.Value += direction * m_D * deltaTime;
            }).Run();
        }
    }
}