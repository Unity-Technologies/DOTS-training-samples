using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(CreatureMovementSystem))]
public partial class CreatureTransformUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        var delta = Time.DeltaTime;

        Entities
            .ForEach((ref Tile tile, ref Direction dir, ref TileLerp lerp, ref Translation trans, ref Rotation rot) =>
            {
                // Translation
                int3 direction = dir.Value.ToVector3();
                float3 start = new float3(tile.Coords, 0);
                trans.Value = math.lerp(start.xzy, start.xzy + direction.xzy, lerp.Value);

                // Rotation
                var targetRotation = dir.Value switch
                {
                    DirectionEnum.North => quaternion.Euler(0,math.PI, 0),
                    DirectionEnum.West => quaternion.Euler(0,math.PI * 0.5f, 0),
                    DirectionEnum.South => quaternion.Euler(0,0, 0),
                    DirectionEnum.East => quaternion.Euler(0,math.PI * -0.5f, 0),
                    _ => rot.Value
                };

                rot.Value = math.slerp(rot.Value, targetRotation, math.saturate(delta * config.CreatureAnimationSpeed));
            }).ScheduleParallel();
        
        

    }
}
