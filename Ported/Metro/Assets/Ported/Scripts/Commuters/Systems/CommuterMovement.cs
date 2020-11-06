#if ENABLE_COMMUTERS
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MetroECS.Comuting
{
    public class CommuterMovement : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            Entities
                .WithAll<MovingTag>()
                .ForEach((ref Translation translation, ref DynamicBuffer<MoveTarget> targets, in Entity commuterEntity, in Commuter commuter) =>
            {
                var currentTarget = targets[0];
                if (math.distance(translation.Value, currentTarget.Position) < 0.01f)
                {
                    targets.RemoveAt(0);
                    
                    if (targets.Length == 0)
                        ecb.RemoveComponent<MovingTag>(commuterEntity);
                }
                else
                {
                    translation.Value = math.lerp(translation.Value, currentTarget.Position, deltaTime * commuter.movementSpeed);
                }
            }).Run();

            ecb.Playback(EntityManager);
        }
    }

    public struct MovingTag : IComponentData
    {
    }
}
#endif