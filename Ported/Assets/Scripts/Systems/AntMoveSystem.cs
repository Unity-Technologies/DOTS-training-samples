using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityMath = Unity.Mathematics;

[UpdateAfter(typeof(SteeringSystem))]
public partial class AntPreMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.WithAll<AntTag>().ForEach((Entity entity, ref AntMovementState antMovementState, in Translation translation, in Velocity velocity) =>
        {
            antMovementState.origin.x = translation.Value.x;
            antMovementState.origin.y = translation.Value.y;
            antMovementState.delta = velocity.Direction * velocity.Speed * deltaTime;
        }).Run();
    }
}

[UpdateAfter(typeof(AntPreMoveSystem))]
public partial class AntCollisionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var obstacleBuffer = GetBuffer<ObstaclePositionAndRadius>(GetSingletonEntity<Grid2D>());
        var grid = GetSingleton<Grid2D>();

        Entities
            .WithAll<AntTag>()
            .WithReadOnly(obstacleBuffer)
            .ForEach((Entity entity, ref CollisionResult collision, in AntMovementState antMovementState) =>
            {
                collision.Reset();
                var result = antMovementState.result;

                // World boundaries collisions
                // all this assume the world is between (-0.5f, -0.5f) and (0.5f, 0.5f)
                if (result.x <= -0.5f)
                {
                    if (result.y < -0.5f)
                    {
                        collision.point = UnityMath.math.float2(-0.5f, -0.5f);
                        collision.normal = UnityMath.math.normalize(new UnityMath.float2(1f, 1f));
                        return;
                    }
                    else if (result.y > 0.5f)
                    {
                        collision.point = UnityMath.math.float2(-0.5f, 0.5f);
                        collision.normal = UnityMath.math.normalize(new UnityMath.float2(1f, -1f));
                        return;
                    }
                    else
                    {
                        collision.normal = UnityMath.math.float2(1f, 0.0f);
                        collision.point.x = -0.5f;
                        collision.point.y = antMovementState.origin.y + UnityMath.math.abs(-0.5f - antMovementState.origin.x) * antMovementState.delta.y / antMovementState.delta.x;
                        return;
                    }
                }
                else if (result.x > 0.5f)
                {
                    if (result.y < -0.5f)
                    {
                        collision.point = UnityMath.math.float2(0.5f, -0.5f);
                        collision.normal = UnityMath.math.normalize(new UnityMath.float2(-1f, 1f));
                        return;
                    }
                    else if (result.y > 0.5f)
                    {
                        collision.point = UnityMath.math.float2(0.5f, 0.5f);
                        collision.normal = UnityMath.math.normalize(new UnityMath.float2(-1f, -1f));
                        return;
                    }
                    else
                    {
                        collision.normal = UnityMath.math.float2(-1f, 0f);
                        collision.point.x = 0.5f;
                        collision.point.y = antMovementState.origin.y + UnityMath.math.abs(0.5f - antMovementState.origin.x) * antMovementState.delta.y / antMovementState.delta.x;
                        return;
                    }
                }
                else if (result.y < -0.5f)
                {
                    collision.normal = UnityMath.math.float2(0f, 1f);
                    collision.point.x = antMovementState.origin.x + UnityMath.math.abs(-0.5f - antMovementState.origin.y) * antMovementState.delta.x / antMovementState.delta.y;
                    collision.point.y = -0.5f;
                    return;
                }
                else if (result.y > 0.5f)
                {
                    collision.normal = UnityMath.math.float2(0f, -1f);
                    collision.point.x = antMovementState.origin.x + UnityMath.math.abs(0.5f - antMovementState.origin.y) * antMovementState.delta.x / antMovementState.delta.y;
                    collision.point.y = 0.5f;
                    return;
                }

                // circular obstacle collisions
                // check if we are inside an obstacle
                var xIdx = (int)math.floor(math.clamp(result.x + 0.5f, 0, 1) * grid.rowLength);
                var yIdx = (int)math.floor(math.clamp(result.y + 0.5f, 0, 1) * grid.columnLength);
                var idx = math.min(xIdx + yIdx * grid.rowLength, obstacleBuffer.Length - 1);
                if (!obstacleBuffer[idx].IsValid)
                {
                    collision.Reset();
                    return;
                }

                // circle-line collision
                var e = antMovementState.origin;
                var center = obstacleBuffer[idx].position;
                var r = obstacleBuffer[idx].radius;
                var d = antMovementState.delta;
                var f = e - center;
                var a = math.dot(d, d);
                var b = 2* math.dot(f, d);
                var c = math.dot(f, f) - r*r;
                var discriminant = math.sqrt(b * b - 4 * a * c);
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                var p1 = e + t1 * d;
                var p2 = e + t2 * d;

                collision.point = math.distancesq(antMovementState.origin,p1) < math.distancesq(antMovementState.origin, p2) ? p1 : p2;
                collision.normal = math.normalize(collision.point - obstacleBuffer[idx].position);
                collision.point += collision.normal * 0.01f;
                return;

            }).Run();
    }
}

[UpdateAfter(typeof(AntCollisionSystem))]
public partial class AntMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.WithAll<AntTag>().ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref Velocity velocity, in CollisionResult collision, in AntMovementState antMovementState) =>
        {
            velocity.Speed = math.max(0.1f, velocity.Speed);
            velocity.Direction = collision.IsValid ? math.normalize(math.reflect(antMovementState.delta, collision.normal)) : math.normalize(velocity.Direction);
            var traslation2D = collision.IsValid ? collision.point + (velocity.Direction * deltaTime * velocity.Speed) : antMovementState.result;
            translation.Value = new float3(traslation2D.x, traslation2D.y , 0);
            rotation.Value = quaternion.LookRotation(new float3(velocity.Direction, 0), new float3(0, 0, 1));
        }).Run();
    }
}
