using Components;
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

            Entities
                .WithAll<KinematicBody>()
                .ForEach((ref Translation translation, ref KinematicBody body) =>
                {
                    if (translation.Value.y > -10f)
                    {
                        body.Velocity.y += deltaY;
                        var newY = (float)math.max(-10f, translation.Value.y - body.Velocity.y);
                        translation.Value.y = newY;
                    }
                }).ScheduleParallel();
        }
    }
}