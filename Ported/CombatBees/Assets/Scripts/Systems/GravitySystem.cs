using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial class GravitySystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _buffer;

        protected override void OnCreate()
        {
            _buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaY = PlayField.gravity * Time.DeltaTime;
            var ecb = _buffer.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithAll<KinematicBody>()
                .WithoutBurst()
                .WithSharedComponentFilter(new KinematicBodyState() { isEnabled = 1 })
                .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref KinematicBody body) =>
                {
                    if (translation.Value.y > body.landPosition.y)
                    {
                        body.velocity.y += deltaY;
                        var newY = (float)math.max(body.landPosition.y, translation.Value.y - body.velocity.y);
                        translation.Value.y = newY;
                    }
                    else
                    {
                        translation.Value.y = body.landPosition.y;
                        body.velocity.y = 0f;
                        // ecb.RemoveComponent<KinematicBody>(entity);
                        ecb.SetSharedComponent(entityInQueryIndex, entity, new KinematicBodyState() { isEnabled = 0 });
                    }
                }).ScheduleParallel();
            
            _buffer.AddJobHandleForProducer(Dependency);
        }
    }
}