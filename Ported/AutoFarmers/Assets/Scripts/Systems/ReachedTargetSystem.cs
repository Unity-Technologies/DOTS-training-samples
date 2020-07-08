using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ReachedTargetSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;
    
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        
            Entities
                .ForEach((int entityInQueryIndex, Entity entity, ref Velocity velocity, in Translation translation, in Target target) =>
                {
                    Translation targetPosition = GetComponent<Translation>(target.Value);
                    float distance = math.distance(targetPosition.Value, translation.Value);
                    if (distance <= 0.01f)
                    {
                        ecb.AddComponent(entityInQueryIndex, entity, new TargetReached());
                    }
                }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }
    }
}