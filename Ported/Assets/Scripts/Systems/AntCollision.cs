using Unity.Collections;
using Unity.Entities;
using UnityMath = Unity.Mathematics;

//public partial class AntCollision : SystemBase
//{
//    protected override void OnUpdate()
//    {
//        var obstacleBuffer = GetBuffer<ObstaclePositionAndRadius>(GetSingletonEntity<Grid2D>());
//        var grid = GetSingleton<Grid2D>();

//        Entities
//            .WithAll<AntTag>()
//            .WithAll<AntMovementState>()
//            .WithAll<CollisionResult>()
//            .ForEach((Entity antEntity) => {
//                var antMovementState = GetComponent<AntMovementState>(antEntity);
//                var collisionResult = GetComponent<CollisionResult>(antEntity);
//                var result = antMovementState.result;

//                // World boundaries collisions
//                // all this assume the world is between (-0.5f, -0.5f) and (0.5f, 0.5f)
//                if(result.x < -0.5f)
//                {
//                    if(result.y < -0.5f)
//                    {
//                        collisionResult.point = UnityMath.math.float2(-0.5f, -0.5f);
//                        collisionResult.normal = UnityMath.math.normalize(new UnityMath.float2(1f, 1f));
//                        return;
//                    }
//                    else if(result.y > 0.5f)
//                    {
//                        collisionResult.point = UnityMath.math.float2(-0.5f, 0.5f);
//                        collisionResult.normal = UnityMath.math.normalize(new UnityMath.float2(1f, -1f));
//                        return;
//                    }
//                    else
//                    {
//                        collisionResult.normal = UnityMath.math.float2(1f, 0);
//                        collisionResult.point.x = -0.5f;
//                        collisionResult.point.y = antMovementState.origin.y + UnityMath.math.abs(-0.5f - antMovementState.origin.x) * antMovementState.delta.y/ antMovementState.delta.x;
//                        return;
//                    }
//                }
//                else if(result.x > 0.5f)
//                {
//                    if (result.y < -0.5f)
//                    {
//                        collisionResult.point = UnityMath.math.float2(0.5f, -0.5f);
//                        collisionResult.normal = UnityMath.math.normalize(new UnityMath.float2(-1f, 1f));
//                        return;
//                    }
//                    else if (result.y > 0.5f)
//                    {
//                        collisionResult.point = UnityMath.math.float2(0.5f, 0.5f);
//                        collisionResult.normal = UnityMath.math.normalize(new UnityMath.float2(-1f, -1f));
//                        return;
//                    }
//                    else
//                    {
//                        collisionResult.normal = UnityMath.math.float2(-1f, 0);
//                        collisionResult.point.x = 0.5f;
//                        collisionResult.point.y = antMovementState.origin.y + UnityMath.math.abs(0.5f - antMovementState.origin.x) * antMovementState.delta.y / antMovementState.delta.x;
//                        return;
//                    }
//                }
//                else if(result.y < -0.5f)
//                {
//                    collisionResult.normal = UnityMath.math.float2(0f, 1f);
//                    collisionResult.point.x = antMovementState.origin.x + UnityMath.math.abs(-0.5f - antMovementState.origin.y) * antMovementState.delta.x / antMovementState.delta.y;
//                    collisionResult.point.y = -0.5f;
//                    return;
//                }
//                else if(result.y > 0.5f)
//                {
//                    collisionResult.normal = UnityMath.math.float2(0f, -1f);
//                    collisionResult.point.x = antMovementState.origin.x + UnityMath.math.abs(0.5f - antMovementState.origin.y) * antMovementState.delta.x / antMovementState.delta.y;
//                    collisionResult.point.y = 0.5f;
//                    return;
//                }

//                // circular obstacle collisions
//                // check if we are inside an obstacle
//                var xIdx = (int)UnityMath.math.floor(result.x * grid.rowLength);
//                var yIdx = (int)UnityMath.math.floor(result.y * grid.columnLength);
//                var idx = xIdx + yIdx * grid.rowLength;
//                if (!obstacleBuffer[idx].IsValid)
//                {
//                    collisionResult.Reset();
//                    return;
//                }

//                // take the point that pass throught the current proposed position
//                collisionResult.normal = UnityMath.math.normalize(result - obstacleBuffer[idx].position);
//                collisionResult.point = obstacleBuffer[idx].position + obstacleBuffer[idx].radius * collisionResult.normal;
//                return;
//            }).Run();
//    }
//}
