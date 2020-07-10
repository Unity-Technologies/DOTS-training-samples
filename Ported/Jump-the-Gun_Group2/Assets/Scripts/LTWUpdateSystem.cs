using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class LTWUpdateSystem : SystemBase
{

    protected override void OnCreate() {

        RequireSingletonForUpdate<GridTag>();
        RequireSingletonForUpdate<GameParams>();

    }

    protected override void OnUpdate()
    {
        var gridtag = GetSingletonEntity<GridTag>();

        DynamicBuffer<GridHeight> gh = EntityManager.GetBuffer<GridHeight>(gridtag);
        GameParams gp = GetSingleton<GameParams>();

        var handles = new NativeArray<JobHandle>(4, Allocator.Temp);

        var localToWorld = GetComponentDataFromEntity<LocalToWorld>();

        handles[0] = Entities
            .WithNone<Color>()
            .WithNativeDisableContainerSafetyRestriction(localToWorld)
            .WithNone<LookAtPlayerTag>()
            .ForEach((Entity e, in Position pos) =>
            {
                var trans = float4x4.Translate(pos.Value);
                var scale = float4x4.Scale(CannonFireSystem.kCannonBallRadius);
                localToWorld[e] = new LocalToWorld { Value = math.mul(trans, scale) };
            }).ScheduleParallel(Dependency);

        handles[1] = Entities
            .WithReadOnly(gh)
            .WithNativeDisableContainerSafetyRestriction(localToWorld)
            .ForEach((Entity e, ref Color c, in Position pos) => {
                var trans = float4x4.Translate(pos.Value);
                float height = gh[GridFunctions.GetGridIndex(pos.Value.xz, gp.TerrainDimensions)].Height;
                var scale = float4x4.Scale(1, height, 1);
                localToWorld[e] = new LocalToWorld { Value = math.mul(trans, scale) };

                float range = (gp.TerrainMax - gp.TerrainMin);
                float value = (height - gp.TerrainMin) / range;
                float4 color = math.lerp(gp.colorA, gp.colorB, value);
                c.Value = color;
            }).ScheduleParallel(Dependency);

        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerLocation = EntityManager.GetComponentData<Position>(playerEntity).Value.xz;

        handles[2] = Entities
            .WithNone<Rotation>()
            .WithNativeDisableContainerSafetyRestriction(localToWorld)
            .WithAll<LookAtPlayerTag>()
            .ForEach((Entity e, in Position pos) => {
                var trans = float4x4.Translate(pos.Value);
                var scale = float4x4.Scale(1);
                var direction = playerLocation - pos.Value.xz;
                var yaw = math.atan(direction.x / direction.y);// direction.y < 0 ? math.PI : 0f;
                if (direction.y < 0)
                    yaw += math.PI;
                var rotate = float4x4.RotateY(yaw);
                localToWorld[e] = new LocalToWorld { Value = math.mul(math.mul(trans, scale), rotate) };                
            }).ScheduleParallel(Dependency);

        handles[3] = Entities
            .WithAll<LookAtPlayerTag>()
            .WithNativeDisableContainerSafetyRestriction(localToWorld)
            .ForEach((Entity e, in Position pos, in Rotation rot) => {
                var trans = float4x4.Translate(pos.Value);
                var scale = float4x4.Scale(1);
                var direction = playerLocation - pos.Value.xz;
                var yaw = math.atan(direction.x / direction.y);// direction.y < 0 ? math.PI : 0f;
                        if (direction.y < 0)
                    yaw += math.PI;
                var rotate = float4x4.EulerXYZ(rot.Value, yaw, 0);
                localToWorld[e] = new LocalToWorld { Value = math.mul(math.mul(trans, scale), rotate) };
            }).ScheduleParallel(Dependency);

        Dependency = JobHandle.CombineDependencies(handles);
    }
}