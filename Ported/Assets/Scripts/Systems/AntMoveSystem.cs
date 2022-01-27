using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityMath = Unity.Mathematics;

[UpdateAfter(typeof(SteeringSystem))]
public partial class AntMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var obstacleBuffer = GetBuffer<ObstaclePositionAndRadius>(GetSingletonEntity<Grid2D>());
        var grid = GetSingleton<Grid2D>();

        Entities.WithAll<AntTag>().ForEach((Entity entity, ref AntMovementState antMovementState, in Translation translation, in Velocity velocity) =>
        {
            antMovementState.origin.x = translation.Value.x;
            antMovementState.origin.y = translation.Value.y;
            antMovementState.delta = velocity.Direction * velocity.Speed * deltaTime;
        }).ScheduleParallel();

        Entities.WithAll<AntTag>().ForEach((Entity entity, ref CollisionResult collision, in AntMovementState antMovementState) =>
        {
            var result = antMovementState.result;

            // World boundaries collisions
            // all this assume the world is between (-0.5f, -0.5f) and (0.5f, 0.5f)
            if (result.x < -0.5f)
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
                    collision.normal = UnityMath.math.float2(1f, 0);
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
                    collision.normal = UnityMath.math.float2(-1f, 0);
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
            var xIdx = (int)math.floor((result.x + 0.5f) * grid.rowLength);
            var yIdx = (int)math.floor((result.y + 0.5f) * grid.columnLength);
            var idx = xIdx + yIdx * grid.rowLength;
            if (!obstacleBuffer[idx].IsValid)
            {
                collision.Reset();
                return;
            }

            // take the point that pass throught the current proposed position
            collision.normal = math.normalize(result - obstacleBuffer[idx].position);
            collision.point = obstacleBuffer[idx].position + obstacleBuffer[idx].radius * collision.normal;
            return;

        }).ScheduleParallel();

        Entities.WithAll<AntTag>().ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref Velocity velocity, in CollisionResult collision,  in AntMovementState antMovementState) =>
        {
            translation.Value = new float3(collision.IsValid ? collision.point : antMovementState.result, 0);
            velocity.Direction = collision.IsValid ? collision.normal : velocity.Direction;
            rotation.Value = quaternion.LookRotation(new float3(velocity.Direction, 0), new float3(0, 0, 1));
        }).ScheduleParallel();
    }
}
