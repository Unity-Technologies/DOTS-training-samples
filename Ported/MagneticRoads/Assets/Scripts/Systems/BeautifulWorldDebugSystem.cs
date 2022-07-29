using Components;
using Unity.Burst;
using Unity.Entities;
using Util;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    partial struct BeautifulWorldDebugSystem : ISystem
    {
        public const int DebugSplineResolution = 5;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var roadSegment in SystemAPI.Query<RefRO<RoadSegment>>())
            {
                var rs = roadSegment.ValueRO;
                var prevPos = Spline.EvaluatePosition(rs.Start, rs.End, 0);
                var prevRot = Spline.EvaluateRotation(rs.Start, rs.End, 0);
                var prevRight2 = math.mul(prevRot, new float3(1, 0, 0)) * 2;
                float3 nextPos;
                quaternion nextRot;
                float3 nextRight2;

                for (int i = 0; i < DebugSplineResolution; i++)
                {
                    var nextT = ((float) i + 1) / DebugSplineResolution;
                    nextPos = Spline.EvaluatePosition(rs.Start, rs.End, nextT);
                    nextRot = Spline.EvaluateRotation(rs.Start, rs.End, nextT);
                    nextRight2 = math.mul(nextRot, new float3(1, 0, 0)) * 2;

                    UnityEngine.Debug.DrawLine(prevPos - prevRight2, nextPos - nextRight2);
                    UnityEngine.Debug.DrawLine(prevPos + prevRight2, nextPos + nextRight2);

                    prevPos = nextPos;
                    prevRight2 = nextRight2;
                }
            }

            foreach (var intersection in SystemAPI.Query<TransformAspect>().WithAll<Intersection>())
            {
                var pos = intersection.Position;
                var fwd2 = intersection.Forward * 2;
                var right2 = intersection.Right * 2;

                // Box
                UnityEngine.Debug.DrawLine(pos + fwd2 - right2, pos + fwd2 + right2);
                UnityEngine.Debug.DrawLine(pos + fwd2 + right2, pos - fwd2 + right2);
                UnityEngine.Debug.DrawLine(pos - fwd2 + right2, pos - fwd2 - right2);
                UnityEngine.Debug.DrawLine(pos - fwd2 - right2, pos + fwd2 - right2);

                // Cross
                UnityEngine.Debug.DrawLine(pos + fwd2, pos - fwd2);
                UnityEngine.Debug.DrawLine(pos + right2, pos - right2);
            }
        }
    }
}
