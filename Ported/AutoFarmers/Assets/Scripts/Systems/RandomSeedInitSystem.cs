using Unity.Entities;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class RandomSeedInitSystem : SystemBase
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
                .ForEach((int entityInQueryIndex, Entity entity, in UsesRandomness randomness) =>
                {
                    ecb.AddComponent(entityInQueryIndex, entity, new RandomSeed() {
                        //NOTE: Seed must be non-zero
                        Value = (((uint) entity.Index) + 1) * 500
                    });
                    ecb.RemoveComponent<UsesRandomness>(entityInQueryIndex, entity);
                }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }
    }
}