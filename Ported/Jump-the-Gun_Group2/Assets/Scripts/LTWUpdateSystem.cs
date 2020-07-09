using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class LTWUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {

        var gridtag = GetSingletonEntity<GridTag>();

        DynamicBuffer<GridHeight> gh = EntityManager.GetBuffer<GridHeight>(gridtag);
        GameParams gp = GetSingleton<GameParams>();

        Entities
            .WithNone<Color>()
            .WithNone<LookAtPlayerTag>()
            .ForEach((ref LocalToWorld localToWorld, in Position pos) =>
            {
                var trans = float4x4.Translate(pos.Value);
                var scale = float4x4.Scale(0.6f);
                localToWorld.Value = math.mul(trans, scale);
            }).ScheduleParallel();

        Entities
            .WithReadOnly(gh)
            .ForEach((ref LocalToWorld localToWorld, ref Color c, in Position pos) => {
                var trans = float4x4.Translate(pos.Value);
                float height = gh[GridFunctions.GetGridIndex(pos.Value.xz, gp.TerrainDimensions)].Height;

                var scale = float4x4.Scale(1, height, 1);
                localToWorld.Value = math.mul(trans, scale);

                float range = (gp.TerrainMax - gp.TerrainMin);
                float value = (height - gp.TerrainMin) / range;
                float4 color = math.lerp(gp.colorA, gp.colorB, value);
                c.Value = color;
            }).ScheduleParallel();

        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerLocation = EntityManager.GetComponentData<Position>(playerEntity).Value.xz;

        Entities
            .WithNone<Rotation>()
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

        Entities
            .WithAll<LookAtPlayerTag>()
            .ForEach((ref LocalToWorld localToWorld, in Position pos, in Rotation rot) => {
                var trans = float4x4.Translate(pos.Value);
                var scale = float4x4.Scale(1);
                var direction = playerLocation - pos.Value.xz;
                var yaw = math.atan(direction.x / direction.y);// direction.y < 0 ? math.PI : 0f;
                        if (direction.y < 0)
                    yaw += math.PI;
                var rotate = float4x4.EulerXYZ(rot.Value, yaw, 0);
                localToWorld.Value = math.mul(math.mul(trans, scale), rotate);
            }).ScheduleParallel();
    }
}