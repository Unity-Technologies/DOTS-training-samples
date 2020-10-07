using Unity.Entities;
using Unity.Mathematics;

public class AIMovement : SystemBase
{
    static readonly float AISpeed = 3f;
    
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var elapsedTime = (uint)System.DateTime.Now.Ticks;
        var boardSize = GetSingleton<GameInfo>().boardSize;
        Entities.ForEach((int entityInQueryIndex, ref AICursor cursor, ref Position position) =>
        {
            var direction = cursor.Destination - position.Value;
            var distance = math.lengthsq(direction);
            var movement = deltaTime * AISpeed * math.normalize(direction);
            var movementLength = math.lengthsq(movement);

            if (distance == 0 || movementLength >= distance)
            {
                position.Value = cursor.Destination;
                var random = new Random((uint)(elapsedTime + entityInQueryIndex));
                cursor.Destination = new int2(random.NextInt(0, boardSize.x), random.NextInt(0, boardSize.y));
                // TODO: click and wait before moving again maybe?
            }
            else
            {
                position.Value += movement;
            }

        }).ScheduleParallel();
    }
}
