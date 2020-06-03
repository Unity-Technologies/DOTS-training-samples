using Unity.Entities;

class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<MoveableTag>()
            .ForEach((ref Position2D pos, ref Rotation2D rot, ref Direction2D dir) =>
            {

            })
            .WithName("UpdateMovables")
            .ScheduleParallel();
    }
}