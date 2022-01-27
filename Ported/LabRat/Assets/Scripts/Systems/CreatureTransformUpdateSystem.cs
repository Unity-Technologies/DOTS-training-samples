using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// make this system run right before the transforms/projections
[UpdateInGroup(typeof(TransformSystemGroup), OrderFirst = true)]
public partial class CreatureTransformUpdateSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Config>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        var delta = Time.DeltaTime;

        Entities
            .WithAll<Creature>()
            .ForEach((ref Direction dir, ref Translation trans, ref Rotation rot, in Tile tile, in TileLerp lerp) =>
            {
                // Translation
                int3 direction = dir.Value.ToVector3();
                float3 start = new float3(tile.Coords, 0);
                trans.Value = math.lerp(start.xzy, start.xzy + direction.xzy, lerp.Value);

                // Rotation
                if (dir.Value.Passable())
                {
                    rot.Value = math.slerp(rot.Value, dir.Value.ToQuaternion(), math.saturate(delta * config.CreatureAnimationSpeed));
                }
            }).ScheduleParallel();
        
        Entities
            .WithAll<Cat>()
            .ForEach((ref Scale scale) =>
            {
                scale.Value = math.lerp(scale.Value, 1.0f, math.saturate(delta * config.CreatureAnimationSpeed));
            }).ScheduleParallel();
    }
}
