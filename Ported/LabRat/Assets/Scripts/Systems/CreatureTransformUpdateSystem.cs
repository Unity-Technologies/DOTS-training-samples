using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(CreatureMovementSystem))]
public partial class CreatureTransformUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp, ref Translation trans, ref Rotation rot) =>
            {
                int3 direction = dir.Value.ToVector3();
                float3 start = new float3(tile.Coords, 0);
                trans.Value = math.lerp(start.xzy, start.xzy + direction.xzy, lerp.Value);
                var dirRotation = dir.Value switch
                {
                    DirectionEnum.North => math.PI,
                    DirectionEnum.West => math.PI * 0.5f,
                    DirectionEnum.South => 0f,
                    DirectionEnum.East => math.PI * -0.5f,
                    _ => 0f
                };
                rot.Value = quaternion.Euler(0, dirRotation, 0);
            }).ScheduleParallel();
        
        

    }
}
