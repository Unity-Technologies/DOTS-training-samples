using Unity.Entities;
using Unity.Mathematics;

public class CannonLookAtSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerLocation = EntityManager.GetComponentData<Position>(playerEntity).Value.xz;

        Entities
            .ForEach((ref Rotation rotation, in Position position) => 
            {
                var direction = playerLocation - position.Value.xz;
                var rot = direction.y < 0 ? math.PI : 0f;
                rotation.Value = rot + math.atan(direction.x / direction.y);
            }).ScheduleParallel();
    }
}