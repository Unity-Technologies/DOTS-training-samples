using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
partial struct TargetSeekingSystem : ISystem
{
    private EntityQuery wallQuery;

    public void OnCreate(ref SystemState state)
    {
        wallQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Obstacle>().Build();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        float3 foodPos = new();
        float3 colonyPos = new();

        foreach (var food in SystemAPI.Query<TransformAspect>().WithAll<Food>())
        {
            foodPos = food.WorldPosition;
        }
        foreach (var colony in SystemAPI.Query<TransformAspect>().WithAll<Colony>())
        {
            colonyPos = colony.WorldPosition;
        }

        var walls = wallQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
        var job = new TargetSeekingJob { foodPos = foodPos, colonyPos = colonyPos, wallPositions = walls};
        job.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(Ant))]
    partial struct TargetSeekingJob : IJobEntity
    {
        public float3 foodPos;
        public float3 colonyPos;

        [ReadOnly]
        public NativeArray<LocalTransform> wallPositions;

        private bool Intersect(float2 p1, float2 p2, float2 center, float radius)
        {
            //  get the distance between X and Z on the segment
            float2 dp = p2 - p1;

            float a = math.dot(dp, dp);
            float b = 2 * math.dot(dp, p1 - center);
            float c = math.dot(center, center) - 2 * math.dot(center, p1) + math.dot(p1, p1) - radius * radius;
            float bb4ac = b * b - 4 * a * c;
            if (math.abs(a) < float.Epsilon || bb4ac < 0)
            {
                //  line does not intersect
                return false;
            }

            float t1 = (-b + math.sqrt(bb4ac)) / (2 * a);
            float t2 = (-b - math.sqrt(bb4ac)) / (2 * a);

            return (t1 > 0.0f && t1 < 1.0f) || (t2 > 0.0f && t2 < 1.0f);
        }

        [BurstCompile]
        public void Execute(ref TargetDirection targetDirection, in CurrentDirection currentDirection, in HasResource hasResource, in LocalTransform transformAspect)
        {
            bool lineOfSight = true;

            float3 targetPos = hasResource.Value ? colonyPos : foodPos;

            foreach (var wall in wallPositions)
            {
                if (Intersect(transformAspect.Position.xz, targetPos.xz, wall.Position.xz, 1.0f))
                {
                    lineOfSight = false;
                    // Draw line to the obstacle
                    // Debug.DrawLine(ant.Item1.WorldPosition, new float3(wall.LocalPosition.x, 0, wall.LocalPosition.z), lineOfSight ? Color.green : Color.red);
                }

            }
            if (lineOfSight)
            {
                targetDirection.Angle = math.atan2(targetPos.x - transformAspect.Position.x, targetPos.z - transformAspect.Position.z);
                targetDirection.Angle = targetDirection.Angle - currentDirection.Angle;
                if (targetDirection.Angle > math.PI)
                    targetDirection.Angle -= (float)(math.PI * 2.0f);
                if (targetDirection.Angle < -math.PI)
                    targetDirection.Angle += (float)(math.PI * 2.0f);
            }
            else
            {
                targetDirection.Angle = 0;
            }
            //Debug.DrawLine(targetPos, new float3(ant.Item1.LocalPosition.x, 0, ant.Item1.LocalPosition.z), lineOfSight ? Color.green : Color.red);
        }
    }
}
