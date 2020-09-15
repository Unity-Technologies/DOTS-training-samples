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
        float deltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref Position position, ref PositionOffset positionOffset, in Direction direction, in Speed speed) => {
            positionOffset.Value += speed.Value * deltaTime;
            if (positionOffset.Value > 1f)
            {
                positionOffset.Value -= 1f;
                var pos = position.Value;
                switch (direction.Value)
                {
                    case DirectionEnum.North:
                        pos.y += 1;
                        break;
                    case DirectionEnum.South:
                        pos.y -= 1;
                        break;
                    case DirectionEnum.East:
                        pos.x += 1;
                        break;
                    case DirectionEnum.West:
                        pos.x -= 1;
                        break;
                }
                position.Value = pos;
            }
        }).ScheduleParallel();
    }
}
