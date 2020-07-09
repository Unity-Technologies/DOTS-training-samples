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
            .WithNone<LookAtPlayerTag>()
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

        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerLocation = EntityManager.GetComponentData<Position>(playerEntity).Value.xz;

        Entities
            .WithAll<LookAtPlayerTag>()
            .ForEach((ref LocalToWorld localToWorld, in Position pos) => {
                var trans = float4x4.Translate(pos.Value);
                var scale = float4x4.Scale(1);
                var direction = playerLocation - pos.Value.xz;
                var yaw = math.atan(direction.x / direction.y);// direction.y < 0 ? math.PI : 0f;
                if (direction.y < 0)
                    yaw += math.PI;
                var rotate = float4x4.RotateY(yaw);
                localToWorld.Value = math.mul(math.mul(trans, scale), rotate);
            }).ScheduleParallel();

    }
}