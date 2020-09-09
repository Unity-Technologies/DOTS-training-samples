using Unity.Entities;


public class AnimalMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        
    }

    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        
        Entities.ForEach((ref PositionXZ pos, in Speed speed) =>
        {
            pos.Value.x += speed.Value * time;
        }).ScheduleParallel();
    }
}
