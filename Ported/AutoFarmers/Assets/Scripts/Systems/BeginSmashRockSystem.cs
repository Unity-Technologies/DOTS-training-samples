using Unity.Entities;

namespace AutoFarmers
{
    class BeginSmashRockSystem : SystemBase
    {
        private EntityCommandBufferSystem ecbSystem;

        protected override void OnCreate()
        {
            ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();

            Entities.WithAll<PathFindingTarget>().WithAll<TargetReached>().WithAll<Attacking>().WithAll<SmashRock_Intent>()
                .ForEach((int entityInQueryIndex, Entity entity) =>
            {
                ecb.RemoveComponent<PathFindingTarget>(entityInQueryIndex, entity);
                ecb.RemoveComponent<TargetReached>(entityInQueryIndex, entity);
                ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
            }).ScheduleParallel();

            ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}