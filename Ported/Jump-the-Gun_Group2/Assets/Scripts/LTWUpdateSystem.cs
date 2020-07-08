using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class LTWUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithNone<Height>()
            .WithNone<Rotation>()
            .ForEach((ref LocalToWorld localToWorld, in Position pos) =>
            {
                var trans = float4x4.Translate(pos.Value);
                var scale = float4x4.Scale(1);
                localToWorld.Value = math.mul(trans, scale);
            }).ScheduleParallel();

        Entities
            .ForEach((ref LocalToWorld localToWorld, in Position pos, in Height height) => {
                var trans = float4x4.Translate(pos.Value);
                var scale = float4x4.Scale(1, height.Value, 1);
                localToWorld.Value = math.mul(trans, scale);
            }).ScheduleParallel();

        Entities
            .ForEach((ref LocalToWorld localToWorld, in Position pos, in Rotation rot) => {
                var trans = float4x4.Translate(pos.Value);
                var scale = float4x4.Scale(1);
                var rotate = float4x4.RotateY(rot.Value);
                localToWorld.Value = math.mul(math.mul(trans, scale), rotate);
            }).ScheduleParallel();

    }
}