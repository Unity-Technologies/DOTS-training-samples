using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref Position position, in Speed speed, in Direction direction) => {
            position.Value += direction.Value * speed.Value * deltaTime;
        }).ScheduleParallel();
    }
}
