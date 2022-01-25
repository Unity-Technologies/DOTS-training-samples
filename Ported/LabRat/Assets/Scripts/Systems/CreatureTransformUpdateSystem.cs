using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CreatureTransformUpdateSystem : SystemBase
{
    private static int3 DirectionToVector(DirectionEnum dir)
    {
        return new int3((dir & DirectionEnum.East) != 0 ? 1 : (dir & DirectionEnum.West) != 0 ? -1 : 0,
            (dir & DirectionEnum.South) != 0 ? 1 : (dir & DirectionEnum.North) != 0 ? -1 : 0,
            (dir & DirectionEnum.Hole) != 0 ? -1 : 0);
    }

    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp, ref Translation trans, ref Rotation rot) =>
            {
                int3 direction = DirectionToVector(dir.Value);
                float3 start = new float3(tile.Coords, 0);
                trans.Value = math.lerp(start.xzy, start.xzy + direction.xzy, lerp.Value);
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
