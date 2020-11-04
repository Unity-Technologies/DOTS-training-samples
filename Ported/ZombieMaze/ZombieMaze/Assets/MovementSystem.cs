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

        var mazeSize = GetSingleton<MazeSize>();
        
        Entities.ForEach((ref Position position, in Speed speed, in Direction direction) => {
            position.Value += direction.Value * speed.Value * deltaTime;
            position.Value = math.clamp(position.Value, -mazeSize.Value/2, mazeSize.Value/2);
        }).ScheduleParallel();
    }
}
