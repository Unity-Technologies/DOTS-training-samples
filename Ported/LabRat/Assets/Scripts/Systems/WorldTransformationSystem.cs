using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MovementSystem))]
public class WorldTransformationSystem : SystemBase
{
    protected override void OnUpdate()
    {      
        
        Entities.ForEach((ref Translation translation, ref Rotation rotation, in Position position, in PositionOffset offset, in Direction direction) => {

            float3 posOffset;
            switch (direction.Value)
            {
                case DirectionEnum.North:
                    posOffset = new float3(0, 0, offset.Value);
                    rotation.Value = quaternion.identity;
                    break;
                case DirectionEnum.South:
                    posOffset = new float3(0, 0, -offset.Value);
                    rotation.Value = quaternion.RotateY(math.PI);
                    break;
                case DirectionEnum.East:
                    posOffset = new float3(offset.Value, 0, 0);
                    rotation.Value = quaternion.RotateY(math.PI / 2f);
                    break;
                case DirectionEnum.West:
                    posOffset = new float3(-offset.Value, 0, 0);
                    rotation.Value = quaternion.RotateY(-math.PI / 2f);
                    break;
                default:
                    posOffset = float3.zero;
                    break;
            }

            translation.Value = new float3(position.Value.x, 0f, position.Value.y) + posOffset;
        }).ScheduleParallel();
    }
}
