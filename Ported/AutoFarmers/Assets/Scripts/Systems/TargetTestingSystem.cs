using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class TargetTestingSystem : SystemBase
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
                .ForEach((int entityInQueryIndex, Entity entity, in Target target, in TargetReached targetReached) =>
                {
                    ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<TargetReached>(entityInQueryIndex, entity);
                }).ScheduleParallel();
        
            Entities
                .WithNone<Target>()
                .ForEach((int entityInQueryIndex, Entity entity, ref RandomSeed randomSeed, in Farmer_Tag tag, in TargetTest targetTest) =>
                {
                    Random random = new Random(randomSeed.Value);
                    int randomInt =  random.NextInt(0,3);
                    randomSeed.Value = random.state;
                    
                    Entity nextTarget;
                    switch (randomInt)
                    {
                        case 0:
                            nextTarget = targetTest.TargetOne;
                            break;
                        case 1:
                            nextTarget = targetTest.TargetTwo;
                            break;
                        case 2:
                            nextTarget = targetTest.TargetThree;
                            break;
                        default:
                            nextTarget = targetTest.TargetOne;
                            break;
                    }

                    ecb.AddComponent(entityInQueryIndex, entity, new Target()
                    {
                        Value = nextTarget
                    });
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
        }
    }
}