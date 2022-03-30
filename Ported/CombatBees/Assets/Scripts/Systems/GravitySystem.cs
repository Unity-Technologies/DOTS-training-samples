using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial class GravitySystem : SystemBase
    {
        private const float Gravity = 4f;

        protected override void OnUpdate()
        {
            var deltaY = Gravity * Time.DeltaTime;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities
                .WithAll<KinematicBody>()
                .ForEach((Entity entity, ref Translation translation, ref KinematicBody body) =>
                {
                    if (translation.Value.y > body.LandPosition.y)
                    {
                        body.Velocity.y += deltaY;
                        var newY = (float)math.max(body.LandPosition.y, translation.Value.y - body.Velocity.y);
                        translation.Value.y = newY;
                    }
                    else
                    {
                        translation.Value.y = body.LandPosition.y;
                        body.Velocity.y = 0f;
                        ecb.RemoveComponent<KinematicBody>(entity);
                    }
                }).Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}