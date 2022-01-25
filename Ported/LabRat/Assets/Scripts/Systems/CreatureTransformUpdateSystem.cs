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
                var direction = new int2((dir.Value & DirectionEnum.East) != 0 ? 1: (dir.Value & DirectionEnum.West) != 0 ? -1 : 0,
                    (dir.Value & DirectionEnum.South) != 0 ? 1 : (dir.Value & DirectionEnum.North) != 0 ? -1 : 0);
                var lerped = math.lerp(tile.Coords, tile.Coords + direction, lerp.Value);
                trans.Value = new float3(lerped.x, 0, lerped.y);
                var dirRotation = dir.Value switch
                {
                    DirectionEnum.North => math.PI,
                    DirectionEnum.East => math.PI * 0.5f,
                    DirectionEnum.South => 0f,
                    DirectionEnum.West => math.PI * -0.5f,
                    _ => 0f
                };
                rot.Value = quaternion.Euler(0, dirRotation, 0);
            }).ScheduleParallel();
        
        

    }
}
