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

            // TODO: refactor to structural changes instead of shared components
            Entities
                .WithAll<KinematicBody>()
                .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref KinematicBody body) =>
                {
                    if (translation.Value.y > body.landPosition.y)
                    {
                        body.velocity.y -= deltaY;
                        var newY = (float)math.max(body.landPosition.y, translation.Value.y - body.velocity.y);
                        translation.Value.y = newY;
                    }
                    else
                    {
                        translation.Value.y = body.landPosition.y;
                        body.velocity.y = 0f;
                        ecb.RemoveComponent<KinematicBody>(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();

            Entities
                .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation,
                    in Components.Resource resource, in ResourceOwner resourceOwner) =>
                {
                    if (resourceOwner.Owner != Entity.Null && 
                    (HasComponent<BeeMovement>(resourceOwner.Owner) &&
                    !HasComponent<BeeLifetime>(resourceOwner.Owner)))
                    {
                        translation.Value = resource.OwnerPosition;
                    }
                    else
                    {
                        ecb.RemoveComponent<ResourceOwner>(entityInQueryIndex, entity);
                        ecb.AddComponent<KinematicBody>(entityInQueryIndex, entity,
                            new KinematicBody()
                            {
                                landPosition = new float3(translation.Value.x, -PlayField.size.y / 2,
                                    translation.Value.z)
                            });
                    }
                }).ScheduleParallel();

            _buffer.AddJobHandleForProducer(Dependency);
        }
    }
}